using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainGame
{
    public class WorldSpaceUi : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Transform Spawner;
        public int displayDistance = 5;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            bool active = (Vector3.Distance(Spawner.position, target.position) < displayDistance);
            Spawner.gameObject.SetActive(active);
        }
    }
}
