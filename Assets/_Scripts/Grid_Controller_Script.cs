using UnityEngine;

[ExecuteInEditMode]
public class Grid_Controller_Script : MonoBehaviour
{

    public float cell_width = 0.12f;
    public float cell_height  = 0.12f;
    public float depth = 0.0f;

    // Max lines in each axis
    public int max_X;
    public int max_Y;
    // 12x12 Grid Settings
    public Color grid_12_colour;
    public bool show_12_grid;
    // 192x192 Grid Settings
    public Color grid_192_colour;
    public bool show_192_grid;
    public int grid_192_mult = 32;
    

    public void Update()
    {
        Grid_Children();
    }

    void Grid_Children()
    {
        // get children transforms
        // apply grid code to each transform

        // get array of all children and grandchildren under grid controller object //
        Transform[] child_transforms = gameObject.transform.GetComponentsInChildren<Transform>();
        // for each transform in this array, snap to a grid value based on the snap value //
        foreach (Transform child_transform in child_transforms)
        {
            SnapToGrid(child_transform);
        }
    }

    void SnapToGrid(Transform child_transform)
    {
        float cell_width_inverse = 1.0f / cell_width;
        float cell_height_inverse = 1.0f / cell_height;

        float x, y, z;
        
        // if cell_width = .1, x = 1.22
        // cell_width_inverse = 10, x * cell_width_inverse = x * 10 = 12.2
        // round (x * cell_width_inv)/ cell_width_inv = 12 / 10 = 1.2
        // therefor 1.22 to the nearest .1 is 1.2
        x = Mathf.Round((child_transform.position.x) * cell_width_inverse) / cell_width_inverse;
        y = Mathf.Round((child_transform.position.y) * cell_height_inverse) / cell_height_inverse;
        z = depth;  // depth from camera
        

        child_transform.position = new Vector3(x, y, z);
    }


    void OnDrawGizmos()
    {
        // if the 12x12 grid should be shown
        if (show_12_grid)
        {
            Gizmos.color = grid_12_colour;

            for (int x = 0; x < max_X; x++)
            {
                // set start and end points, recentred around (0,0)
                Vector3 start = new Vector3(x - max_X / 2, -max_Y / 2) * cell_width;
                Vector3 end = new Vector3(x - max_X / 2, max_Y / 2) * cell_width;
                Gizmos.DrawLine(start, end);
            }
            for (int y = 0; y < max_Y; y++)
            {
                Vector3 start = new Vector3(0 - max_X / 2, y - max_Y / 2) * cell_height;
                Vector3 end = new Vector3(max_X / 2, y - max_Y / 2) * cell_height;
                Gizmos.DrawLine(start, end);
            }

            // Psuedocode
            // for every x value
            // draw line from (x, 0) to (x, max_y)
            // for every y value
            // draw line from (0,y) to (max_X, y)
        }
        // if the 192 grid should be shown
        if (show_192_grid)
        {
            Gizmos.color = grid_192_colour;

            for (int x = 0; x < max_X; x++)
            {
                // set start and end points, recentred around (0,0)
                Vector3 start = new Vector3(x - max_X / 2, -max_Y / 2) * cell_width * grid_192_mult;
                Vector3 end = new Vector3(x - max_X / 2, max_Y / 2) * cell_width * grid_192_mult;
                Gizmos.DrawLine(start, end);
            }
            for (int y = 0; y < max_Y; y++)
            {
                Vector3 start = new Vector3(0 - max_X / 2, y - max_Y / 2) * cell_height * grid_192_mult;
                Vector3 end = new Vector3(max_X / 2, y - max_Y / 2) * cell_height * grid_192_mult;
                Gizmos.DrawLine(start, end);
            }
        }
    }


}


