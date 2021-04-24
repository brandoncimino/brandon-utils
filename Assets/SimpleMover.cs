using UnityEngine;

public class SimpleMover : MonoBehaviour {
    public float Speed = 10;

    public RectTransform.Axis Style = RectTransform.Axis.Vertical;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() {
        var haxis = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        var vaxis = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        transform.position +=
            new Vector3(
                haxis * Time.deltaTime * Speed,
                Style == RectTransform.Axis.Horizontal ? 0 : vaxis * Time.deltaTime * Speed,
                Style == RectTransform.Axis.Horizontal ? vaxis * Time.deltaTime * Speed : 0
            );
    }
}
