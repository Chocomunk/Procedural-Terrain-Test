using UnityEngine;
using System.Collections;

public class SquirrelSee : MonoBehaviour {

	private SquirrelInfo info;

	// Use this for initialization
	void Start () {
		info = this.GetComponent<SquirrelInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		GameObject targ = null;
		float distance = 0;
		
		for(float a=-info.eyeAngleOffset; a<2*info.eyeAngleOffset; a+=2*info.eyeAngleOffset){
			for(float i=0; i<180; i+=info.sightClarity){
				float rad = (i+a)*Mathf.Deg2Rad;
				float radius = (info.sightScale*Mathf.Sin(rad))/(Mathf.Abs(90-((i)%180))/info.transformFade + info.transformSmooth);
				
				Vector3 sightLine = this.transform.rotation*(new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)));
				
				float range = 0;
				RaycastHit hit;
				if(Physics.Raycast(this.transform.position, sightLine, out hit, radius, 1<<8)){
					range = hit.distance;
					SquirrelInfo itsInfo = hit.transform.GetComponent<SquirrelInfo>();
					if((targ == null || distance < hit.distance)&&info.canMate&&itsInfo.canMate&&(itsInfo.hasYC ^ info.hasYC)){
						targ = hit.collider.gameObject;
						distance = hit.distance;
					}
				}else{
					range = radius;
				}

				Debug.DrawLine(this.transform.position, this.transform.position + range*sightLine);
			}
		}
		
		SetTarget(targ);
	}

	void SetTarget(GameObject target){
		info.target = target;
		this.GetComponent<SquirrelMove>().setTarget(target);
	}

//	void OnDrawGizmos(){
//		Gizmos.color = Color.red;
//		for(float a=-info.eyeAngleOffset; a<2*info.eyeAngleOffset; a+=2*info.eyeAngleOffset){
//			for(float i=0; i<180; i+=info.sightClarity){
//				float rad = (i+a)*Mathf.Deg2Rad;
//				float radius = (info.sightScale*Mathf.Sin(rad))/(Mathf.Abs(90-((i)%180))/info.transformFade + info.transformSmooth);
//				
//				Vector3 sightLine = this.transform.rotation*(new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)));
//				Vector3 target = this.transform.position + radius*sightLine;
//				
//				Gizmos.DrawLine(this.transform.position,target);
//			}
//		}
//	}
}
