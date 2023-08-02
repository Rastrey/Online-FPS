using Mirror;
using UnityEngine;

public class NetTransform : NetworkBehaviour
{
	[SerializeField] private Vector3 _serverPosition;
	[SerializeField] private float _percent = 0.05f;
	[SerializeField] private float _bias = 1;
	private Rigidbody _playerRb;
	private Transform _playerTransfrom;


	private void Awake()
	{
		_playerRb = GetComponent<Rigidbody>();
		_playerTransfrom = _playerRb.transform;
	}

	private void FixedUpdate()
	{
		if (isServer)
		{
			RpcSetPosition(transform.position);
			RpcSetVelocity(_playerRb.velocity);
			RpcSetRotation(_playerTransfrom.rotation);
		}
		if (isClientOnly)
		{
			transform.position = Vector3.Lerp(transform.position, _serverPosition, _percent * Mathf.Pow(Vector3.Distance(transform.position, _serverPosition) + _bias, 2f));
		}
	}

	[ClientRpc]
	private void RpcSetPosition(Vector3 newPosition)
	{
		_serverPosition = newPosition;
	}

	[ClientRpc]
	private void RpcSetRotation(Quaternion newRotation)
	{
		transform.rotation = newRotation;
	}

	[ClientRpc]
	private void RpcSetVelocity(Vector3 newVelocity)
	{
		_playerRb.velocity = newVelocity;
	}
}
