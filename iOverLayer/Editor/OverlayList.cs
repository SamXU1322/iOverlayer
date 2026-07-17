using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class OverlayList
    {
        private ScrollView _list;
        private CanvasArea _canvas;

        public void Bind(VisualElement root, CanvasArea canvas)
        {
            _list = root.Q<ScrollView>("overlay-list");
            _canvas = canvas;
            _canvas.OverlaysChanged += Rebuild;
            _canvas.SelectionChanged += OnSelectionChanged;
            Rebuild();
        }

        public void Unbind()
        {
            if (_canvas != null)
            {
                _canvas.OverlaysChanged -= Rebuild;
                _canvas.SelectionChanged -= OnSelectionChanged;
            }
            _list = null;
            _canvas = null;
        }

        private void OnSelectionChanged(VisualElement _) => Rebuild();

        public void Rebuild()
        {
            if (_list == null) return;
            _list.Clear();

            var selected = _canvas.SelectedLabel;
            foreach (var label in _canvas.GetLabels())
                _list.Add(BuildRow(label, label == selected));
        }

        private VisualElement BuildRow(Label label, bool isActive)
        {
            var data = LabelData.Of(label);
            bool visible = label.style.display != DisplayStyle.None;

            var row = new VisualElement();
            row.AddToClassList("row-item");
            if (isActive) row.AddToClassList("active");

            var dot = new VisualElement();
            dot.AddToClassList("dot");
            dot.style.backgroundColor = label.resolvedStyle.color;
            row.Add(dot);

            var nameLabel = new Label(data.name);
            nameLabel.AddToClassList("name");
            row.Add(nameLabel);

            var nameField = new TextField { value = data.name };
            nameField.AddToClassList("name-edit");
            nameField.style.display = DisplayStyle.None;
            row.Add(nameField);

            nameLabel.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.button == 0 && evt.clickCount >= 2)
                {
                    nameLabel.style.display = DisplayStyle.None;
                    nameField.SetValueWithoutNotify(data.name);
                    nameField.style.display = DisplayStyle.Flex;
                    nameField.Focus();
                    nameField.SelectAll();
                    evt.StopPropagation();
                }
                else if (evt.button == 1)
                {
                    evt.StopPropagation();
                }
            });
            nameField.RegisterCallback<BlurEvent>(_ => CommitRename(label, nameLabel, nameField));
            nameField.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                {
                    CommitRename(label, nameLabel, nameField);
                    evt.StopPropagation();
                }
                else if (evt.keyCode == KeyCode.Escape)
                {
                    nameField.style.display = DisplayStyle.None;
                    nameLabel.style.display = DisplayStyle.Flex;
                    evt.StopPropagation();
                }
            });

            var lockBtn = new Button();
            lockBtn.AddToClassList("row-btn");
            SvgIconLoader.ApplyToButton(lockBtn, "icon-lock", data.locked ? new Color(0.96f, 0.62f, 0.04f) : new Color(0.39f, 0.45f, 0.52f));
            if (data.locked) lockBtn.AddToClassList("on");
            lockBtn.tooltip = data.locked ? "解锁" : "锁定";
            lockBtn.clicked += () => _canvas.SetLabelLocked(label, !data.locked);
            row.Add(lockBtn);

            // Visibility toggle
            var eyeBtn = new Button();
            eyeBtn.AddToClassList("row-btn");
            SvgIconLoader.ApplyToButton(eyeBtn, "icon-eye", visible ? new Color(0.8f, 0.84f, 0.88f) : new Color(0.2f, 0.26f, 0.33f));
            if (!visible) eyeBtn.AddToClassList("off");
            eyeBtn.tooltip = visible ? "隐藏" : "显示";
            eyeBtn.clicked += () => _canvas.SetLabelVisible(label, !visible);
            row.Add(eyeBtn);

            // Delete
            var delBtn = new Button { text = "×" };
            delBtn.AddToClassList("row-btn");
            delBtn.AddToClassList("danger");
            delBtn.tooltip = "删除";
            delBtn.clicked += () => _canvas.DeleteLabel(label);
            row.Add(delBtn);

            // Click row to select (locked labels can't be selected)
            // Skip nameLabel so double-click can reach it for rename
            row.RegisterCallback<ClickEvent>(evt =>
            {
                if (evt.clickCount > 1) return;
                if (evt.target is Button) return;
                if (evt.target == nameLabel || evt.target == nameField) return;
                if (nameField.style.display == DisplayStyle.Flex) return;
                if (!data.locked) _canvas.SelectLabel(label);
            });

            return row;
        }

        private void CommitRename(Label label, Label nameLabel, TextField nameField)
        {
            if (nameField.style.display == DisplayStyle.None) return;
            var newName = string.IsNullOrWhiteSpace(nameField.value) ? nameLabel.text : nameField.value.Trim();
            nameField.style.display = DisplayStyle.None;
            nameLabel.text = newName;
            nameLabel.style.display = DisplayStyle.Flex;
            _canvas.RenameLabel(label, newName);
        }
    }
}
