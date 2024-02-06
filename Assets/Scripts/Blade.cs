using UnityEngine;

public class Blade  
{
	private BladeGenericData genericData;
	private Vector2 previousPosition;
	private Transform bladeTrail;
	private Vector3 lastVel;
	private Camera cam;

	public enum BladeState {WAIT,CUTTING}
	private BladeState currentState;

	private WaitForSecondsRealtime comboTextWait;
	private PlayerData playerData;
	private Vector3 comboOffsett3D;
	private float lastHitTime;
	
	public Blade (BladeGenericData _genericData, PlayerData _playerData, GameObject bladeTrailPrefab, Camera _raycastCamera) 
	{
		genericData = _genericData;
		playerData = _playerData;
		cam = _raycastCamera;
		currentState = BladeState.WAIT;
		comboOffsett3D = new Vector3(GameData.Instance.ComboTextOffset.x,GameData.Instance.ComboTextOffset.y,1);
		bladeTrail = GameObject.Instantiate(bladeTrailPrefab).transform;

		playerData.score = 0;
	}
	
	private void Slice(Rigidbody body, Vector3 direction, float force) 
    {
        float dotVelDir = Vector3.Dot(body.velocity,direction);
        bool rotFlip = dotVelDir > 0; //SHOULD CALCULATE USING EXIT POINT LOCATION NOT DIRECTION

        Vector3 directionUnit = direction * dotVelDir;

        float perpUnitMaginute = (body.velocity - directionUnit).magnitude + force;
        //directionUnit += direction*force;

		FruitData fruitData = body.transform.GetComponent<FruitData>();

        fruitData.colliderEnabled = false;
		body.constraints = RigidbodyConstraints.FreezeAll;
        body.transform.eulerAngles = Vector3.forward * (Vector3.SignedAngle(Vector3.up,directionUnit,Vector3.forward));


        Rigidbody body0 = fruitData.firstBody;
        Rigidbody body1 = fruitData.secondBody;

        bool flip = Vector3.Dot(body1.transform.localPosition,bladeTrail.right) < 0;

		int depthToAdd = GameData.Instance.GameManager.CurrentFRound.FruitCount * GameData.Instance.DistanceBetweenLayers;

        body0.transform.SetParent(null);
		body0.gameObject.SetActive(true);
		body0.transform.position += Vector3.forward * depthToAdd;


        body0.velocity = directionUnit + body.transform.right * perpUnitMaginute * (flip ? -genericData.HighPerpRatio : genericData.LowPerpRatio);
        body0.AddTorque((rotFlip?-1:1)* direction  * perpUnitMaginute * (flip ? -genericData.HighPerpRatio : genericData.LowPerpRatio), ForceMode.VelocityChange);
        
        body1.transform.SetParent(null);
		body1.gameObject.SetActive(true);
		body1.transform.position += Vector3.forward * depthToAdd;

        body1.velocity = directionUnit + body.transform.right * perpUnitMaginute * (flip ? -genericData.LowPerpRatio : genericData.HighPerpRatio );
		body1.AddTorque((rotFlip?-1:1)* direction * perpUnitMaginute * (flip ? -genericData.LowPerpRatio : genericData.HighPerpRatio ), ForceMode.VelocityChange);
        
		GameObject.Destroy(body.gameObject);
		GameObject.Destroy(body0.gameObject,5);
		GameObject.Destroy(body1.gameObject,5);

    }

	private void UpdateCut (Vector3 inputPosition)
	{

		Vector2 newPosition = cam.ScreenToWorldPoint(inputPosition);
        bladeTrail.position = newPosition;

		lastVel = (newPosition - previousPosition) * Time.deltaTime;

		if (lastVel.magnitude > genericData.MinCuttingVelocity)
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (inputPosition);
			Physics.Raycast(ray,out hit,100);
		
			if (hit.collider != null) 
			{
				HandleCollision(hit.collider.gameObject);
			}

			
		} 

		previousPosition = newPosition;
		
	}

	private void StartCutting (Vector3 inputPosition)
	{
		previousPosition = cam.ScreenToWorldPoint(inputPosition);
		bladeTrail.position = previousPosition;
		bladeTrail.GetComponent<TrailRenderer>().Clear();
	}

	private void HandleCollision(GameObject hitObj) 
	{
		FruitData fruitData = hitObj.GetComponent<FruitData>();

		if (fruitData == null) return;
		
		char mark = fruitData.mark;
		if (mark == '-') 
		{
			playerData.score -=2;
			playerData.combo = 0;

			float squareExplosionDistance = GameData.Instance.BombDestructionRange * GameData.Instance.BombDestructionRange;
			foreach (Transform t in GameData.Instance.GameManager.SpawnedFruits) 
			{
				if (t==null) continue;
							
				Vector3 surfaceDistance = hitObj.transform.position - t.position;
				surfaceDistance.z  = 0;

				if (surfaceDistance.sqrMagnitude <= squareExplosionDistance)
				GameObject.Destroy(t.gameObject);
			}
					
			GameObject.Destroy(hitObj);
					
		}

		else 
		{
			fruitData.explode = true;
				
			if (mark == GameData.Instance.GameManager.CurrentFRound.CorrectMark) 
			{
				SFXManager.Instance.PlaySFX("correct");
				playerData.score++;
				playerData.combo++;
				fruitData.correctParticle = true;
				GameData.Instance.GameManager.CurrentFRound.IsSuccess = true;
				playerData.hasSucceedAtIndex = Questions.Index;

				playerData.comboTextPosition = hitObj.transform.position + comboOffsett3D;
				lastHitTime = Time.time;
			}

			else 
			{
				SFXManager.Instance.PlaySFX("wrong");
				playerData.score--;
				playerData.combo = 0;
				playerData.wrongParticle.Fire();
			}	

			Slice(hitObj.GetComponent<Rigidbody>(),lastVel.normalized,2f);		
		}
				
	}

	public BladeState CurrentBladeState() 
	{
		return currentState;
	}
	public void BladeWithTouch(bool isInput, Vector3 inputPosition) 
	{
		if (isInput) 
		{
			RaycastHit hit;
			Ray ray = cam.ScreenPointToRay (inputPosition);
			Physics.Raycast(ray,out hit,100);
		
			if (hit.collider != null) 
			{
				lastVel = Random.insideUnitCircle;
				HandleCollision(hit.collider.gameObject);
			}
		}
	}
 	public void UpdateBlade(bool isInput, Vector3 inputPosition) 
	{
		
		switch (currentState) 
        {
			case BladeState.WAIT:

				if (isInput) 
				{
					StartCutting(inputPosition);
					currentState = BladeState.CUTTING;
				}
					
			break;

			case BladeState.CUTTING:

				if(!isInput) 
				{
					bladeTrail.GetComponent<TrailRenderer>().Clear();
					currentState = BladeState.WAIT;
				}
					
				else 
				{
					UpdateCut(inputPosition);
				}

			break;
		}
	}

	public void ComboReset() => playerData.combo = (Time.time - lastHitTime >= GameData.Instance.ComboMaxDelayError) ? 0 : playerData.combo;


}
