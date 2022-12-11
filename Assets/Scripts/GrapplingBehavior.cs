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
    private bool prevClicked = false;
    private StarterAssetsInputs _input;
    private LineRenderer _lr;

    private Vector3 grapplingPos;

    private Transform _camera, _playerPos;

    
    public GrapplingBehavior(StarterAssetsInputs inputs, LineRenderer lr, Transform plyr,Transform cmra){
        _input = inputs;
        _lr = lr;
        _camera = cmra;
        _playerPos = plyr;
    }
    
    public Vector3 grapple(){
        RaycastHit hit;
        if(Physics.Raycast(_camera.position, _camera.forward, out hit, 100f)){
            grapplingPos = hit.point;
        }
        if(_input.grapple){
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, grapplingPos);
            Debug.Log("grappled to " + grapplingPos);
            return Vector3.Normalize(grapplingPos);
        }
        else{
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, _playerPos.position);
            return Vector3.zero;
        }
    }
    

}




/*

while(clicking grappling button){
    wherever hooked, constantly add velocity in that direction




}
*/