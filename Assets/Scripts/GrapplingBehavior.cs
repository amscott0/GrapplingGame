using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    using UnityEngine.InputSystem;
#endif

public class GrapplingBehavior
{
    bool prevClicked = false;
    StarterAssetsInputs _input;

    // Start is called before the first frame update
    public GrapplingBehavior(StarterAssetsInputs inputs){
        _input = inputs;
    }
    // Update is called once per frame
    public Vector3 grapple(){
        Debug.Log(_input.grapple);
        return Vector3.zero;
        
    }
    

}




/*

while(clicking grappling button){
    wherever hooked, constantly add velocity in that direction




}
*/