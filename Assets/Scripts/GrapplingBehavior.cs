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
    bool noPointFound = true;
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
        if(_input.grapple){
            
            if(!prevClicked){
                prevClicked = true;
                RaycastHit hit;
                if(Physics.Raycast(_camera.position, _camera.forward, out hit, 100f)){
                    grapplingPos = hit.point;
                    noPointFound = true;
                }
                else{
                    noPointFound = false;
                }
            }
            if(!noPointFound){
                _lr.SetPosition(0, _playerPos.position);
                _lr.SetPosition(1, _playerPos.position);
                return Vector3.zero;
            }
            
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, grapplingPos);
            Debug.Log("grappled to " + grapplingPos);

            float magnitudeMult = MathF.Sqrt(Vector3.Distance(_playerPos.position, grapplingPos));

            return (-magnitudeMult) *6* Vector3.Normalize(_playerPos.position - grapplingPos);
        }
        else{
            _lr.SetPosition(0, _playerPos.position);
            _lr.SetPosition(1, _playerPos.position);

            prevClicked = false;
            // grapplingPos = _playerPos.position;

            return Vector3.zero;
        }
    }
    

}




/*

while(clicking grappling button){
    wherever hooked, constantly add velocity in that direction




}
*/