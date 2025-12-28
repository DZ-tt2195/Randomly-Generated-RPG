using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

[System.Serializable]
public class UploadedFile
{
    public string fileName;
    public string fileText;
}
public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Button uploadButton;
    [SerializeField] Button downloadButton;
    [SerializeField] TextAsset toDownload;

    void Start()
    {
        uploadButton.gameObject.SetActive(false);
        downloadButton.gameObject.SetActive(false);

        dropdown.onValueChanged.AddListener(ChangeLanguageDropdown);

        List<string> languages = Translator.inst.GetTranslations().Keys.ToList();
        for (int i = 0; i < languages.Count; i++)
        {
            string nextLanguage = languages[i];
            dropdown.AddOptions(new List<string>() { nextLanguage });
            if (nextLanguage.Equals(PlayerPrefs.GetString("Language")))
            {
                dropdown.value = i;
                ChangeLanguageDropdown(i);
            }
        }

        void ChangeLanguageDropdown(int n)
        {
            Translator.inst.ChangeLanguage(dropdown.options[dropdown.value].text, null);
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        {
            uploadButton.gameObject.SetActive(true);
            downloadButton.gameObject.SetActive(true);
            downloadButton.onClick.AddListener(DownloadBaseFile);
            uploadButton.onClick.AddListener(OpenFilePicker);
        }
        #endif
    }

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void UploadTextFile(string gameObjectName, string callbackMethod);
    #endif

    void OpenFilePicker()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        UploadTextFile(this.gameObject.name, nameof(OnFileLoaded));
    #endif
    }

    public void OnFileLoaded(string json)
    {
        UploadedFile file = JsonUtility.FromJson<UploadedFile>(json);
        Dictionary<string, string> newDictionary = Translator.ReadLanguageFile(file.fileText);
        Translator.inst.ChangeLanguage(file.fileName, newDictionary);
    }

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadTsvFile(string filename, string text);
    #endif

    void DownloadBaseFile()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        DownloadTsvFile("English File.tsv", toDownload.text);
    #endif
    }
}
