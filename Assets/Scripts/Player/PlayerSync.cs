using Mirror;
using UnityEngine;

namespace Player
{
	public class PlayerSync : PlayerMovement
	{
		private bool _isJumpPressed;


		private new void Update()
		{
			base.Update();
			if (!isLocalPlayer)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				ChangeJumpState(true);
			}
			if (Input.GetKeyUp(KeyCode.Space))
			{
				ChangeJumpState(false);
			}
		}

		private new void FixedUpdate()
		{
			base.FixedUpdate();
			if (isLocalPlayer && isClientOnly)
			{
				if ((transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical")).y != 0)
                {
					print(0);
                }
				CmdSetInputDir(transform.right * Input.GetAxisRaw("Horizontal") + transform.forward * Input.GetAxisRaw("Vertical"));
				CmdSetRotation(_rotation);
			}
			
			if (isClientOnly || isLocalPlayer) return;

			if (!OnGround)
			{
				AirAccelerate();
				return;
			}
			if (_isJumpPressed)
			{
                if (_jumpStamina > 0)
				{
					Jump();
				}
				return;
			}
			ServerGroundAccelerate();
			Friction();
		}

		[Server]
		private void ServerGroundAccelerate()
		{
			GroundAccelerate();
		}

		[Command]
		private void CmdSetRotation(Vector2 rotation)
		{
			transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
		}

		[Command]
		private void CmdSetInputDir(Vector3 inputDir)
		{
			_moveDir = inputDir;
			_moveDir.Normalize();
			if (!OnGround) return;
			_moveDir = Vector3.Cross(Vector3.Cross(_groundNormal, _moveDir), _groundNormal);
		}

		[Command]
		private void ChangeJumpState(bool newState)
		{
			_isJumpPressed = newState;
		}
	}
}
