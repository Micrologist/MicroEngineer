using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DockedWindowController : MonoBehaviour
{
    public VisualElement root;
    public VisualElement secondWindow;
    public GameObject myGameObject;

    private bool isActive = false;
    private string activeStyleClass = "active";

    private void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        secondWindow = root.Q<VisualElement>("root2");

        Debug.Log("Root grabbed: " + root.name);
        root.transform.position = new Vector3(800, 500, 0);

        var title = root.Q<VisualElement>("title");
        title.RegisterCallback<MouseDownEvent>(OnClick);

        var title2 = secondWindow.Q<VisualElement>("title2");
        title2.RegisterCallback<MouseDownEvent>(OnClick);


        // Example: Change the text of a label element
        //Label label = visualElement.Q<Label>("myLabel");
        //label.text = "Hello, UI Builder!";
    }

    private void OnClick(MouseDownEvent evt)
    {
        isActive = !isActive;

        //var windowTitle = root.Q<Label>("window-name");
        //Debug.Log("windowTitle: " + windowTitle.text);

        if (isActive)
        {
            var title = evt.currentTarget as VisualElement;
            //var title = root.Q<VisualElement>("title");

            Debug.Log("title grabbed: " + title.name);
            title.AddToClassList("window-title__active");

            var titleArrowDown = title.Q<VisualElement>("title-arrow-down");
            var titleArrowRight = title.Q<VisualElement>("title-arrow-right");
            titleArrowDown.style.display = DisplayStyle.Flex;
            titleArrowRight.style.display = DisplayStyle.None;

            VisualElement body;
            if (title.name == "title")
                body = root.Q<VisualElement>("body");
            else
                body = root.Q<VisualElement>("body2");

            body.style.display = DisplayStyle.Flex;

            Debug.Log(".window-title__active set \n body hidden");
        }
        else
        {
            var title = evt.currentTarget as VisualElement;
            //var title = root.Q<VisualElement>("title");

            Debug.Log("title grabbed" + title.name);
            title.RemoveFromClassList("window-title__active");

            var titleArrowDown = title.Q<VisualElement>("title-arrow-down");
            var titleArrowRight = title.Q<VisualElement>("title-arrow-right");
            titleArrowDown.style.display = DisplayStyle.None;
            titleArrowRight.style.display = DisplayStyle.Flex;

            //var body = root.Q<VisualElement>("body");

            VisualElement body;
            if (title.name == "title")
                body = root.Q<VisualElement>("body");
            else
                body = root.Q<VisualElement>("body2");

            body.style.display = DisplayStyle.None;

            Debug.Log(".window-title__active unset \n body shown");
        }
    }
}