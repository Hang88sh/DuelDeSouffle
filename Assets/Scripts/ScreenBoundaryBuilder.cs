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
        float zDistance = Mathf.Abs(targetCamera.transform.position.z);

        //coins visibles de l'ecran en coordonnees mode
        Vector3 bottomLeft=targetCamera.ScreenToWorldPoint(new Vector3(0,0,zDistance));
        Vector3 topRight=targetCamera.ScreenToWorldPoint(new Vector3(Screen.width,Screen.height,zDistance));
        
        float width=topRight.x-bottomLeft.x;
        float height=topRight.y-bottomLeft.y;
        float centerX = (topRight.x + bottomLeft.x) / 2f;
        float centerY = (topRight.y + bottomLeft.y) / 2f;

        //creer les 4 murs invisibles
        CreateWall("murHaut", new Vector3(centerX, topRight.y + wallThickness / 2f, 0), new Vector3(width + 2f, wallThickness, wallDepth));
        CreateWall("murBas", new Vector3(centerX, bottomLeft.y - wallThickness / 2f, 0), new Vector3(width + 2f, wallThickness, wallDepth));
        CreateWall("murGauche", new Vector3(bottomLeft.x-wallThickness/2f,centerY,0), new Vector3(wallThickness,height + 2f, wallDepth));
        CreateWall("murDroit", new Vector3(topRight.x+wallThickness/2f,centerY,0), new Vector3(wallThickness, height + 2f, wallDepth));

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
