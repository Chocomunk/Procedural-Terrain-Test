using UnityEngine;
using System.Collections;

public class SquirrelInfo : MonoBehaviour {

	[Header("Sight Varuables:")]
	public float sightScale = 120;
	public float sightClarity = 4;
	public float transformFade = 4;
	public float transformSmooth = 4;
	public float eyeAngleOffset = 80;

	[Header("Movement Variables:")]
	public float maxRange = 8;
	public float minRange = 3f;
	public float restRange = 0.25f;

	public float moveChance = 0.1f;
	public float minMoveSpeed = 0.5f;
	public float maxMoveSpeed = 3;
	public float rotateSpeed = 3f;

	[Header("Reproduction Variables:")]
	public GameObject target = null;
	public bool canMate = false;
	public bool hasYC = false;
	public float reproduceCooldown = 5;
	public float birthCooldown = 10;
	public float randomnizerRange = 0.5f;
	public float moveChanceScaler = 0.01f;
	public float sightClarityScaler = 1;
	public float cooldownScaler = 2;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
