using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

// Modified by Mogencelab on Aug 09 2018. (for sheet name filtering etc)
// Original source from https://github.com/Benzino/ExcelToJsonConverter
public class ExcelToJsonConverterWindow : EditorWindow 
{
	public static string kExcelToJsonConverterInputPathPrefsName = "ExcelToJson.InputPath";
	public static string kExcelToJsonConverterOuputPathPrefsName = "ExcelToJson.OutputPath";
    public static string kExcelToJsonConverterSheetNamePrefsName = "ExcelToJson.SheetName";
    public static string kExcelToJsonConverterModifiedFilesOnlyPrefsName = "ExcelToJson.OnlyModifiedFiles";
    public static string kExcelToJsonConverterSeparateFilePerSheetPrefsName = "ExcelToJson.SeparateFilePerSheet";

    private string _inputPath;
	private string _outputPath;
    private string _sheetName;
	private bool _onlyModifiedFiles;
    private bool _separateFilePerSheet;

	private ExcelToJsonConverter _excelProcessor;

	[MenuItem ("Tools/Excel To Json Converter")]
	public static void ShowWindow() 
	{
		EditorWindow.GetWindow(typeof(ExcelToJsonConverterWindow), true, "Excel To Json Converter", true);
	}

	public void OnEnable()
	{
		if (_excelProcessor == null)
		{
			_excelProcessor = new ExcelToJsonConverter();
		}

		_inputPath = EditorPrefs.GetString(kExcelToJsonConverterInputPathPrefsName, Application.dataPath);
		_outputPath = EditorPrefs.GetString(kExcelToJsonConverterOuputPathPrefsName, Application.dataPath);
        _sheetName = EditorPrefs.GetString(kExcelToJsonConverterSheetNamePrefsName, "");
		_onlyModifiedFiles = EditorPrefs.GetBool(kExcelToJsonConverterModifiedFilesOnlyPrefsName, false);
        _separateFilePerSheet = EditorPrefs.GetBool(kExcelToJsonConverterSeparateFilePerSheetPrefsName, true);

    }
	
	public void OnDisable()
	{
		EditorPrefs.SetString(kExcelToJsonConverterInputPathPrefsName, _inputPath);
        EditorPrefs.SetString(kExcelToJsonConverterOuputPathPrefsName, _outputPath);
        EditorPrefs.SetString(kExcelToJsonConverterSheetNamePrefsName, _sheetName);
        EditorPrefs.SetBool(kExcelToJsonConverterModifiedFilesOnlyPrefsName, _onlyModifiedFiles);
        EditorPrefs.SetBool(kExcelToJsonConverterSeparateFilePerSheetPrefsName, _separateFilePerSheet);
    }

	void OnGUI()
	{
		GUILayout.BeginHorizontal();

		GUIContent inputFolderContent = new GUIContent("Input Folder", "Select the folder where the excel files to be processed are located.");
		EditorGUIUtility.labelWidth = 120.0f;
		EditorGUILayout.TextField(inputFolderContent, _inputPath, GUILayout.MinWidth(120), GUILayout.MaxWidth(500));
		if (GUILayout.Button(new GUIContent("Select Folder"), GUILayout.MinWidth(80), GUILayout.MaxWidth(100)))
		{
			_inputPath = EditorUtility.OpenFolderPanel("Select Folder with Excel Files", _inputPath, Application.dataPath);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		GUIContent outputFolderContent = new GUIContent("Output Folder", "Select the folder where the converted json files should be saved.");
		EditorGUILayout.TextField(outputFolderContent, _outputPath, GUILayout.MinWidth(120), GUILayout.MaxWidth(500));
		if (GUILayout.Button(new GUIContent("Select Folder"), GUILayout.MinWidth(80), GUILayout.MaxWidth(100)))
		{
			_outputPath = EditorUtility.OpenFolderPanel("Select Folder to save json files", _outputPath, Application.dataPath);
		}
		
		GUILayout.EndHorizontal();

        GUIContent sheetNameContent = new GUIContent("Sheet Name", "If you want to export specified sheet only, type sheet name on this field. (exports every sheets if empty)");
        _sheetName = EditorGUILayout.TextField(sheetNameContent, _sheetName, GUILayout.MinWidth(120), GUILayout.MaxWidth(500));
        
        GUIContent separateFilePerSheetContent = new GUIContent("Seprate File Per Sheets", "If checked, every sheet datas will be saved in single file.");
        _separateFilePerSheet = EditorGUILayout.Toggle(separateFilePerSheetContent, _separateFilePerSheet);

        EditorGUI.BeginDisabledGroup(!_separateFilePerSheet);
            GUIContent modifiedToggleContent = new GUIContent("Modified Files Only", "If checked, only excel files which have been newly added or updated since the last conversion will be processed.");
		    _onlyModifiedFiles = EditorGUILayout.Toggle(modifiedToggleContent, _onlyModifiedFiles);
        EditorGUI.EndDisabledGroup();

		if (string.IsNullOrEmpty(_inputPath) || string.IsNullOrEmpty(_outputPath))
		{
			GUI.enabled = false;
		}

        if (GUILayout.Button("Convert Excel Files"))
        {
            _excelProcessor.ConvertExcelFilesToJson(_inputPath, _outputPath, _onlyModifiedFiles, _sheetName, _separateFilePerSheet);

            AssetDatabase.Refresh();
        }

		GUI.enabled = true;
	}
}