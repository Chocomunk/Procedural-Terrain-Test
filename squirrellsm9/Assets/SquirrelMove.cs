using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SquirrelInfo))]
public class SquirrelMove : MonoBehaviour {

	private Vector3 destination;
	private SquirrelInfo info;

	// Use this for initialization
	void Start () {
		destination = this.transform.position;
		info = this.GetComponent<SquirrelInfo>();
	}
	
	// Update is called once per frame
	void Update () {
		if(inRange()){
			resetTarget();
		}else{
			if(info.target != null){
				setTarget(info.target);
			}
			float speed = Random.Range(info.minMoveSpeed,info.maxMoveSpeed);
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
			                                           Quaternion.LookRotation(destination-this.transform.position),
			                                           info.rotateSpeed*Time.deltaTime/Mathf.Clamp(Vector3.Distance(
																						this.transform.position,
																						destination),0,1));

			this.transform.position += speed*Time.deltaTime*this.transform.forward;
		}
	}

	public void resetTarget(){
		info.target = null;
		float randMove = Random.Range(0,100);
		if(randMove<=info.moveChance){
			ChooseDestination();
		}
	}

	public void setTarget(GameObject o){
		destination = info.target.transform.position;
	}

	void ChooseDestination(){
		float angle = Random.Range(0,Mathf.PI*2);
		float radius = Random.Range(info.minRange,info.maxRange);

		float xcoord = radius*Mathf.Sin(angle);
		float zcoord = radius*Mathf.Cos(angle);

		destination = new Vector3(this.transform.position.x+xcoord,
		                          this.transform.position.y,
		                          this.transform.position.z+zcoord);
	}

	bool inRange(){
		return (Vector3.Distance(new Vector3(this.transform.position.x,0,this.transform.position.z),
		                         new Vector3(destination.x,0,destination.z)) < info.restRange);
	}
}
