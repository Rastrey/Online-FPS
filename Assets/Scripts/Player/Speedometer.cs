using Player;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
	private TextMeshProUGUI _speedometerText;
	private Rigidbody _playerRb;
	private PlayerMovement _playerMovement;


	private void SetVariables(PlayerMovement playerMovement)
	{
		_speedometerText = GameObject.Find("Speedometer Text").GetComponent<TextMeshProUGUI>();
		_playerRb = playerMovement.GetComponent<Rigidbody>();
		_playerMovement = playerMovement;
	}

	private void Start()
	{
		NetManager.Singleton.OnAddPlayerEvent += SetVariables;
	}

	private void Update()
	{
		if (!_playerRb)
		{
			return;
		}
		Vector3 vector = new Vector3(_playerRb.velocity.x, _playerRb.velocity.y, _playerRb.velocity.z);
		if (!_playerMovement.OnGround)
		{
			vector.y = 0f;
		}
		_speedometerText.text = vector.magnitude.ToString();
	}

}
