using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MelonLoader;
using iOverlayer.Config;

namespace iOverlayer.UI
{
    public class EditorUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;

        // Top bar
        private Button _btnNew;
        private Button _btnOpen;
        private Button _btnSave;
        private Button _btnClose;
        private Label _lblFilename;

        // Toolbar
        private Button _toolText;
        private Button _toolSelect;

        // Property panel
        private VisualElement _noSelection;
        private VisualElement _propFields;
        private TextField _propText;
        private FloatField _propX;
        private FloatField _propY;
        private IntegerField _propFontSize;
        private TextField _propColor;
        private VisualElement _colorSwatch;
        private DropdownField _propFont;
        private Toggle _propEnabled;

        // Status bar
        private Label _selInfo;
        private Label _posInfo;
        private Label _countInfo;

        private enum Tool { Text, Select }

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            BindElements();
            RegisterCallbacks();
            SetActiveTool(_toolText);
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void BindElements()
        {
            // Top bar
            _btnNew = _root.Q<Button>("btn-new");
            _btnOpen = _root.Q<Button>("btn-open");
            _btnSave = _root.Q<Button>("btn-save");
            _btnClose = _root.Q<Button>("btn-close");
            _lblFilename = _root.Q<Label>("lbl-filename");

            // Toolbar
            _toolText = _root.Q<Button>("tool-text");
            _toolSelect = _root.Q<Button>("tool-select");

            // Property panel
            _noSelection = _root.Q<VisualElement>("no-selection");
            _propFields = _root.Q<VisualElement>("prop-fields");
            _propText = _root.Q<TextField>("prop-text");
            _propX = _root.Q<FloatField>("prop-x");
            _propY = _root.Q<FloatField>("prop-y");
            _propFontSize = _root.Q<IntegerField>("prop-fontSize");
            _propColor = _root.Q<TextField>("prop-color");
            _colorSwatch = _root.Q<VisualElement>("color-swatch");
            _propFont = _root.Q<DropdownField>("prop-font");
            _propEnabled = _root.Q<Toggle>("prop-enabled");

            // Status bar
            _selInfo = _root.Q<Label>("sel-info");
            _posInfo = _root.Q<Label>("pos-info");
            _countInfo = _root.Q<Label>("count-info");
        }

        private void RegisterCallbacks()
        {
            // Top bar buttons
            if (_btnNew != null)
                _btnNew.clicked += OnNewClicked;
            if (_btnOpen != null)
                _btnOpen.clicked += OnOpenClicked;
            if (_btnSave != null)
                _btnSave.clicked += OnSaveClicked;
            if (_btnClose != null)
                _btnClose.clicked += OnCloseClicked;

            // Tool buttons
            if (_toolText != null)
                _toolText.clicked += OnToolTextClicked;
            if (_toolSelect != null)
                _toolSelect.clicked += OnToolSelectClicked;

            // Property fields
            if (_propText != null)
                _propText.RegisterValueChangedCallback(OnPropTextChanged);
            if (_propX != null)
                _propX.RegisterValueChangedCallback(OnPropXChanged);
            if (_propY != null)
                _propY.RegisterValueChangedCallback(OnPropYChanged);
            if (_propFontSize != null)
                _propFontSize.RegisterValueChangedCallback(OnPropFontSizeChanged);
            if (_propColor != null)
                _propColor.RegisterValueChangedCallback(OnPropColorChanged);
            if (_propFont != null)
                _propFont.RegisterValueChangedCallback(OnPropFontChanged);
            if (_propEnabled != null)
                _propEnabled.RegisterValueChangedCallback(OnPropEnabledChanged);
        }

        private void UnregisterCallbacks()
        {
            if (_btnNew != null)
                _btnNew.clicked -= OnNewClicked;
            if (_btnOpen != null)
                _btnOpen.clicked -= OnOpenClicked;
            if (_btnSave != null)
                _btnSave.clicked -= OnSaveClicked;
            if (_btnClose != null)
                _btnClose.clicked -= OnCloseClicked;

            if (_toolText != null)
                _toolText.clicked -= OnToolTextClicked;
            if (_toolSelect != null)
                _toolSelect.clicked -= OnToolSelectClicked;

            if (_propText != null)
                _propText.UnregisterValueChangedCallback(OnPropTextChanged);
            if (_propX != null)
                _propX.UnregisterValueChangedCallback(OnPropXChanged);
            if (_propY != null)
                _propY.UnregisterValueChangedCallback(OnPropYChanged);
            if (_propFontSize != null)
                _propFontSize.UnregisterValueChangedCallback(OnPropFontSizeChanged);
            if (_propColor != null)
                _propColor.UnregisterValueChangedCallback(OnPropColorChanged);
            if (_propFont != null)
                _propFont.UnregisterValueChangedCallback(OnPropFontChanged);
            if (_propEnabled != null)
                _propEnabled.UnregisterValueChangedCallback(OnPropEnabledChanged);
        }

        // ── Top bar handlers ──

        private void OnNewClicked()
        {
            MelonLogger.Msg("Editor: 新建 JSON");
        }

        private void OnOpenClicked()
        {
            var filePath = ShowFilePicker("选择 JSON 文件", "*.json");
            if (!string.IsNullOrEmpty(filePath))
            {
                MelonLogger.Msg($"Editor: 打开文件 → {filePath}");
                ConfigManager.EnsureConfigDirectory();
            }
        }

        private static string ShowFilePicker(string title, string fileTypeFilter)
        {
            var dialog = (IFileOpenDialog)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")));
            try
            {
                var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                IShellItem initialFolder;
                SHCreateItemFromParsingName(docsPath, IntPtr.Zero, typeof(IShellItem).GUID, out initialFolder);

                dialog.SetOptions(FOS.FORCEFILESYSTEM | FOS.FILEMUSTEXIST | FOS.DONTADDTORECENT);
                dialog.SetTitle(title);
                dialog.SetFileName("");
                dialog.SetOkButtonLabel("打开");

                var filter = new COMDLG_FILTERSPEC { pszName = "JSON Files", pszSpec = fileTypeFilter };
                dialog.SetFileTypes(1, ref filter);

                if (initialFolder != null)
                    dialog.SetFolder(initialFolder);

                if (dialog.Show(IntPtr.Zero) != 0)
                    return null;

                dialog.GetResult(out IShellItem result);
                result.GetDisplayName(SIGDN.FILESYSPATH, out string path);
                return path;
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        private void OnSaveClicked()
        {
            MelonLogger.Msg("Editor: 保存");
        }

        private void OnCloseClicked()
        {
            var gameSceneName = MainUI.GameSceneName;
            AudioListener.pause = false;

            if (!string.IsNullOrEmpty(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
            }
        }


        private void OnToolTextClicked()
        {
            SetActiveTool(_toolText);
            MelonLogger.Msg("Editor: 切换到文字工具");
        }

        private void OnToolSelectClicked()
        {
            SetActiveTool(_toolSelect);
            MelonLogger.Msg("Editor: 切换到选择工具");
        }

        private void SetActiveTool(Button active)
        {
            _toolText?.RemoveFromClassList("active");
            _toolSelect?.RemoveFromClassList("active");
            active?.AddToClassList("active");
        }

        // ── Property change handlers ──

        private void OnPropTextChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 文字内容 → {evt.newValue}");
        }

        private void OnPropXChanged(ChangeEvent<float> evt)
        {
            MelonLogger.Msg($"Editor: X → {evt.newValue}");
        }

        private void OnPropYChanged(ChangeEvent<float> evt)
        {
            MelonLogger.Msg($"Editor: Y → {evt.newValue}");
        }

        private void OnPropFontSizeChanged(ChangeEvent<int> evt)
        {
            MelonLogger.Msg($"Editor: 字号 → {evt.newValue}");
        }

        private void OnPropColorChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 颜色 → {evt.newValue}");
            UpdateColorSwatch(evt.newValue);
        }

        private void OnPropFontChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 字体 → {evt.newValue}");
        }

        private void OnPropEnabledChanged(ChangeEvent<bool> evt)
        {
            MelonLogger.Msg($"Editor: 显示 → {evt.newValue}");
        }

        private void UpdateColorSwatch(string colorHex)
        {
            if (_colorSwatch == null) return;
            if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
            {
                _colorSwatch.style.backgroundColor = new StyleColor(c);
            }
        }

        // ── Win32 COM file dialog types ──

        [ComImport, Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show(IntPtr parent);
            void SetFileTypes(int cFileTypes, [In] ref COMDLG_FILTERSPEC rgFilterSpec);
            void SetFileTypeIndex(int iFileType);
            void GetFileTypeIndex(out int piFileType);
            void Advise(IntPtr pfde, out int pdwCookie);
            void Unadvise(int dwCookie);
            void SetOptions(FOS fos);
            void GetOptions(out FOS pfos);
            void SetDefaultFolder(IShellItem psi);
            void SetFolder(IShellItem psi);
            void GetFolder(out IShellItem ppsi);
            void GetCurrentSelection(out IShellItem ppsi);
            void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetFileName([Out, MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
            void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            void GetResult(out IShellItem ppsi);
            void AddPlace(IShellItem psi, int alignment);
            void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
            void Close(int hr);
            void SetClientGuid(ref Guid guid);
            void ClearClientData();
            void SetFilter(IntPtr pFilter);
        }

        [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
            void GetParent(out IShellItem ppsi);
            void GetDisplayName(SIGDN sigdnName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
            void GetAttributes(int sfgaoMask, out int psfgaoAttribs);
            void Compare(IShellItem psi, int hint, out int piOrder);
        }

        private enum FOS : uint
        {
            OVERWRITEPROMPT = 0x00000002,
            STRICTFILETYPES = 0x00000004,
            NOCHANGEDIR = 0x00000008,
            PICKFOLDERS = 0x00000020,
            FORCEFILESYSTEM = 0x00000040,
            ALLNONSTORAGEITEMS = 0x00000080,
            NOVALIDATE = 0x00000100,
            ALLOWMULTISELECT = 0x00000200,
            PATHMUSTEXIST = 0x00000800,
            FILEMUSTEXIST = 0x00001000,
            CREATEPROMPT = 0x00002000,
            SHAREAWARE = 0x00004000,
            NOREADONLYRETURN = 0x00008000,
            NOTESTFILECREATE = 0x00010000,
            HIDEMRUPLACES = 0x00020000,
            HIDEPINNEDPLACES = 0x00040000,
            NODEREFERENCELINKS = 0x00100000,
            DONTADDTORECENT = 0x02000000,
            FORCESHOWHIDDEN = 0x10000000,
            DEFAULTNOMINIMODE = 0x20000000,
            FORCEPREVIEWPANEON = 0x40000000,
            SUPPORTSTREAMABLEITEMS = 0x80000000
        }

        private enum SIGDN : uint
        {
            NORMALDISPLAY = 0,
            PARENTRELATIVEPARSING = 0x80018001,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            PARENTRELATIVEEDITING = 0x80031001,
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            FILESYSPATH = 0x80058000,
            URL = 0x80068000,
            PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            PARENTRELATIVE = 0x80080001
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct COMDLG_FILTERSPEC
        {
            [MarshalAs(UnmanagedType.LPWStr)] public string pszName;
            [MarshalAs(UnmanagedType.LPWStr)] public string pszSpec;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IShellItem ppv);
    }
}
