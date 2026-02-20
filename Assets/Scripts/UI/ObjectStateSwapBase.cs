using UnityEngine;

[ExecuteAlways]
public abstract class ObjectStateSwapBase : MonoBehaviour, IObjectStateSwaps
{
    [SerializeField] protected int editorState;
    protected int lastEditorState;

    public abstract void SetState(int state);


#if UNITY_EDITOR
    [ExecuteAlways]
    private void Update()
    {
        if (lastEditorState != editorState)
        {
            SetDebugStateEditor(editorState);
        }
    }

    private void SetDebugStateEditor(int state)
    {
        lastEditorState = state;
        editorState = state;
        SetState(state);
    }
#endif
}