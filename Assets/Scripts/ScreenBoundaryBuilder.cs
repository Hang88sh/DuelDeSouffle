using UnityEngine;

public class ScreenBoundaryBuilder : MonoBehaviour
{
    [Header("Cam¨¦ra cible(la cam¨¦ra principale par d¨¦faut)")]
    public Camera targetCamera;

    [Header("Param¨¨tres des murs")]
    public float wallThickness = 1f;//epaisseur des murs
    public float wallDepth = 10f;//Profondeur(en Z)

    public PhysicsMaterial bounceMaterial;

    private void Start()
    {
        //utiliser la camera principale si aucune camera n'est assingee
        if(targetCamera == null)
            targetCamera= Camera.main;

        //Distance a laquelle les murs seront positionnes
        float zWorld = 0f;
        float distanceFromCamera = Mathf.Abs(zWorld - targetCamera.transform.position.z);


        //coins visibles de l'ecran en coordonnees mode
        Vector3 bottomLeft = targetCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceFromCamera));
        Vector3 topRight= targetCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceFromCamera));

        float width=topRight.x-bottomLeft.x;
        float height=topRight.y-bottomLeft.y;
        float centerX = (topRight.x + bottomLeft.x) / 2f;
        float centerY = (topRight.y + bottomLeft.y) / 2f;

        float bottomWallThickness = 0.5f;

        //creer les 4 murs invisibles
        CreateWall("murBas", new Vector3(centerX, bottomLeft.y + bottomWallThickness / 2f-0.31f, zWorld), new Vector3(width + 2f, bottomWallThickness, wallDepth));
        CreateWall("murHaut", new Vector3(centerX, topRight.y + wallThickness / 2f, zWorld), new Vector3(width + 2f, wallThickness, wallDepth));
        CreateWall("murGauche", new Vector3(bottomLeft.x - wallThickness / 2f, centerY, zWorld), new Vector3(wallThickness, height + 2f, wallDepth));
        CreateWall("murDroit", new Vector3(topRight.x + wallThickness / 2f, centerY, zWorld), new Vector3(wallThickness, height + 2f, wallDepth));

    }

    void CreateWall(string name,Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name=name;
        wall.transform.position=position;
        wall.transform.localScale = scale;

        //rendre le mur invisible
        wall.GetComponent<Renderer>().enabled=false;

        //appliquer le materiau de rebond si fourni
        if(bounceMaterial != null)
        {
            Collider collider= wall.GetComponent<Collider>();
            collider.material=bounceMaterial;
        }

        wall.transform.parent=this.transform;
    }
}
