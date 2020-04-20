using UnityEngine;
using System;
using UnityEditor;

/// <summary>
///  Source: http://www.ennoble-studios.com/tuts/making-unitys-www-class-work-in-editor-scripts/
/// </summary>

// Form Data
//WWWForm wwwForm = new WWWForm();
//wwwForm.AddField("myField", myField);
//EditorWWW editorWWW = new EditorWWW();
//editorWWW.StartWWW(_webURL, wwwForm, MyResultOperation);

// Without Form Data
//EditorWWW editorWWW = new EditorWWW();
//editorWWW.StartWWW(_webURL, null, resultWWW);

// In implementation class
// void resultWWW(string result, object[] resultObjs){ ... handle response ...}
//

public class EditorWWW {
    private WWW _www;
    private Action<string, object[]> _operateWWWResult;
    private object[] _arguments;

    public void StartWWW(string path, WWWForm form, Action<string, object[]> operateWWWResult, params object[] arguments) {
        _operateWWWResult = operateWWWResult;
        _arguments = arguments;
        if(form != null) {
            _www = new WWW(path, form);
        } else {
            _www = new WWW(path);
        }
        EditorApplication.update += Tick;
    }

    private void Tick() {
        if(_www.isDone) {
            EditorApplication.update -= Tick;
            if(!string.IsNullOrEmpty(_www.error)) {
                Debug.Log("Error during WWW process:\n" + _www.error);
            } else {
                if(_operateWWWResult != null) _operateWWWResult(_www.text, _arguments);
            }
            _www.Dispose();
        }
    }

    private void StopWWW() {
        EditorApplication.update -= Tick;
        if (_www != null) _www.Dispose();
    }
}
