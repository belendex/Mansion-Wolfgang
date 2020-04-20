using System.Collections.Generic;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.SceneControl.Map
{
    public class MapController : MonoBehaviour {

        public List<MapLocation>  mapLocations;

        public void activateLocation(string locationToActivateName, bool activate) {

            MapLocation locationToActivate = mapLocations.Find(lc => lc.gameObject.name == locationToActivateName);
            locationToActivate.setActive(activate);
        }
    }
}
