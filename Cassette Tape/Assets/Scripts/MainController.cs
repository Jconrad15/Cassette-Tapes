using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CassetteProject;

namespace CassetteProject
{
    public class MainController : MonoBehaviour
    {
        [SerializeField]
        private CassetteGenerator cassetteGen;
        private List<GameObject> currentCassette_gos = new List<GameObject>();

        private int cassetteCount = 9;

        void Start()
        { GenerateNewCassettes(); }

        private void Update()
        { if (Input.GetMouseButtonDown(1)) { GenerateNewCassettes(); } }

        public void GenerateNewCassettes()
        {
            // Destroy cassettes
            while (currentCassette_gos.Count > 0)
            {
                Destroy(currentCassette_gos[0]);
                currentCassette_gos.RemoveAt(0);
            }

            int x = 0;
            int y = 0;
            // Create new cassettes
            for (int i = 0; i < cassetteCount; i++)
            {
                Cassette newCassette = cassetteGen.GenerateCassetteDecal();
                GameObject newCassette_go = cassetteGen.CreateCassetteObject(newCassette);
                newCassette_go.transform.position = new Vector3(x * 0.3f, y * 0.21f, 0);

                y++;
                if (y == 3)
                {
                    x++;
                    y = 0;
                }

                currentCassette_gos.Add(newCassette_go);
            }
        }

    }
}