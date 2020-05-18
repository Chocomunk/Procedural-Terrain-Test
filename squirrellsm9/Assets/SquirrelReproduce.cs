using UnityEngine;
using System.Collections;

public class SquirrelReproduce : MonoBehaviour {
	
	public float moveSpeedScaler = 0.5f;
	public float rotateSpeedScaler = 0.5f;
	public float sightScaleScaler = 40;
	public float moveRangeScaler = 2;

	public Transform prefab;

	private float cooldown = 1f;

	private SquirrelInfo info = null;

	// Use this for initialization
	void Start () {
		info = this.GetComponent<SquirrelInfo>();
	}

	public void setBirth(){
		this.cooldown = info.birthCooldown;
	}

	// Update is called once per frame
	void Update () {
		if(cooldown > 0){
			cooldown -= Time.deltaTime;
		}else if(cooldown < 0){
			cooldown = 0;
			this.info.canMate = true;
		}
	}

	public void resetCollided(){
		info.canMate = false;
		this.cooldown = info.reproduceCooldown;
	}

	void OnCollisionEnter(Collision collision){
		SquirrelInfo thatInfo = collision.gameObject.GetComponent<SquirrelInfo>();
		if(thatInfo.gameObject.layer==this.gameObject.layer && info.canMate && thatInfo.canMate && 
		   (thatInfo.hasYC ^ info.hasYC) && (info.target == thatInfo.gameObject || thatInfo.target == this.gameObject)){
			resetCollided();
			thatInfo.GetComponent<SquirrelReproduce>().resetCollided();

			if(thatInfo.target == this.gameObject){
				thatInfo.GetComponent<SquirrelMove>().resetTarget();
			}
			if(info.target == thatInfo.gameObject){
				this.GetComponent<SquirrelMove>().resetTarget();
			}

			if(!info.hasYC){
				Transform child = GameObject.Instantiate(prefab);
				SquirrelInfo itsInfo = child.GetComponent<SquirrelInfo>();
//				SquirrelInfo itsInfo = new SquirrelInfo();
//				child.gameObject.SetActive(false);

				float randRange = (info.randomnizerRange+thatInfo.randomnizerRange)/2;
				float moveScalar = (info.moveChanceScaler+thatInfo.moveChanceScaler)/2;
				float clarityScaler = (info.sightClarityScaler+thatInfo.sightClarityScaler)/2;
				float coolScalar= (info.cooldownScaler+thatInfo.cooldownScaler)/2;

				float num1 = Random.Range(-randRange,randRange)*moveRangeScaler + (info.maxRange+thatInfo.maxRange)/2;
				float num2 = Random.Range(-randRange,randRange)*moveRangeScaler + (info.minRange+thatInfo.minRange)/2;
				if(num1>=num2){itsInfo.maxRange=num1; itsInfo.minRange=num2;}
				else{itsInfo.minRange=num1; itsInfo.maxRange=num2;}

				num1 = Random.Range(-randRange,randRange)*moveSpeedScaler + (info.maxMoveSpeed+thatInfo.maxMoveSpeed)/2;
				num2 = Random.Range(-randRange,randRange)*moveSpeedScaler + (info.minMoveSpeed+thatInfo.minMoveSpeed)/2;
				if(num1>=num2){itsInfo.maxMoveSpeed=num1; itsInfo.minMoveSpeed=num2;}
				else{itsInfo.minMoveSpeed=num1; itsInfo.maxMoveSpeed=num2;}

				itsInfo.sightScale = Random.Range(-randRange,randRange)*sightScaleScaler + (info.sightScale+thatInfo.sightScale)/2;
				itsInfo.sightClarity = Random.Range(-randRange,randRange)*clarityScaler + (info.sightClarity+thatInfo.sightClarity)/2;
				itsInfo.moveChance = Random.Range(-randRange,randRange)*moveScalar + (info.moveChance+thatInfo.moveChance)/2;
				itsInfo.rotateSpeed = Random.Range(-randRange,randRange)*rotateSpeedScaler + (info.rotateSpeed+thatInfo.rotateSpeed)/2;
				itsInfo.reproduceCooldown = Random.Range(-randRange,randRange)*coolScalar + (info.reproduceCooldown+thatInfo.reproduceCooldown)/2;
				itsInfo.birthCooldown = Random.Range(-randRange,randRange)*coolScalar + (info.birthCooldown+thatInfo.birthCooldown)/2;
				itsInfo.moveChanceScaler = moveScalar + Random.Range(-randRange,randRange)*moveScalar;
				itsInfo.sightClarityScaler = clarityScaler + Random.Range(-randRange,randRange)*clarityScaler;
				itsInfo.cooldownScaler = coolScalar + Random.Range(-randRange,randRange)*coolScalar;
				itsInfo.randomnizerRange = randRange + Random.Range(-randRange,randRange)*randRange;
				itsInfo.hasYC = Random.Range(0f,1f)<0.5;

//				child.gameObject.SetActive(true);
				child.gameObject.GetComponent<SquirrelReproduce>().setBirth();
			}
		}
	}
}