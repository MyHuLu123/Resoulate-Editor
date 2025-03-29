using UnityEngine;
using UnityEngine.UI;

public class Quit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button Quit = gameObject.GetComponent<Button>();
        Quit.onClick.AddListener(QuitMenu);
    }
    private void QuitMenu()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
