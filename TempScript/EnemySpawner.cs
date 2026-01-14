#pragma warning disable
using UnityEngine;
using PrimeTween;

public class EnemySpawner : MonoBehaviour {	
	[Header("Spawn settings")]
	public GameObject enemyPrefab;
	public Transform player;
	public float spawnInterval = 1.5F;
	public float minDistanceFromPlayer = 5F;
	public int maxAttempts = 20;
	[Header("Map size")]
	public float mapWidth = 50F;
	public float mapHeight = 50F;
	private float timer;
	
	private void Update() {
		timer += Time.deltaTime;
		if((timer >= spawnInterval)) {
			timer = 0F;
			SpawnEnemy();
		}
	}
	
	private void SpawnEnemy() {
		var spawnPosition = Vector3.zero;
		var foundPosition = false;
		for(int i = 0; i < maxAttempts; i += 1) {
			var x = Random.Range((-mapWidth / 2F), (mapWidth / 2F));
			var z = Random.Range((-mapHeight / 2F), (mapHeight / 2F));
			spawnPosition = (this.transform.position + new Vector3(x, 0F, z));
			var distanceToPlayer = Vector3.Distance(new Vector3(player.position.x, 0F, player.position.z), new Vector3(spawnPosition.x, 0F, spawnPosition.z));
			if((distanceToPlayer >= minDistanceFromPlayer)) {
				foundPosition = true;
				break;
			}
		}
		if(foundPosition) {
			Tween.Scale(Object.Instantiate<GameObject>(enemyPrefab, spawnPosition, Quaternion.identity).transform, Vector3.zero, Vector3.one, 0.5F, Easing.Standard(Ease.OutBounce), 1, default(CycleMode), default(float), default(float), default(bool));
		}
	}
	
	private void OnDrawGizmosSelected() {
		//Границы карты
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(this.transform.position, new Vector3(mapWidth, 0.1F, mapHeight));
		//Минимальная дистанция от игрока
		if((player != null)) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
		}
	}
}

