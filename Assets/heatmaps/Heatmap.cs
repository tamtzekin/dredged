    // Alan Zucconi
    // www.alanzucconi.com
    using UnityEngine;
    using System.Collections;
     
    public class Heatmap : MonoBehaviour
    {
        Vector4[] positions;
        Vector4[] properties;
     
        public Material material;
     
        int count;

        public LudumInventory inventory; 

        [HideInInspector]
        public bool heatmapRefresh = false;
     
        void Start ()
        {

            count = inventory.gameSettings.startWidth * inventory.gameSettings.startHeight;
            positions = new Vector4[count];
            properties = new Vector4[count];
     
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new Vector4(Random.Range(-0.4f, +0.4f), Random.Range(-0.4f, +0.4f), 0, 0);
                properties[i] = new Vector4(Random.Range(0f, 0.25f), Random.Range(-0.25f, 1f), 0, 0);
            }
        }
     
        void Update()
        {

            if (heatmapRefresh){

                UpdateHeatmap();
                heatmapRefresh = false;            

            }

            for (int i = 0; i < positions.Length; i++)
                positions[i] += new Vector4(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f), 0, 0) * Time.deltaTime;
     
            material.SetInt("_Points_Length", count);
            material.SetVectorArray("_Points", positions);
            material.SetVectorArray("_Properties", properties);
        }

        void UpdateHeatmap(){

            count = inventory.gameSettings.startWidth * inventory.gameSettings.startHeight;
            positions = new Vector4[count];
            properties = new Vector4[count];

            int i = 0;

            for (int y = 0; y < inventory.gameSettings.startHeight; y++){
                for (int x = 0; x < inventory.gameSettings.startWidth; x++){    
                    Debug.Log(inventory.currentScores[x,y]); 
                    if (inventory.currentScores[x,y] > 0){ 
                        positions[i] = new Vector4(x, y, 0,0);
                        properties[i] = new Vector4(1, inventory.currentScores[x,y], 0 ,0);


                        i++; 

       
                    }


                }                

            }

        }
    }
