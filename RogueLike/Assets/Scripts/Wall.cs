using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour
{
	public Sprite dmgSprite;
	public int hp=4;
	private SpriteRenderer spriteRenderer;
	public AudioClip playerChop1;				//1 of 2 Audio clips to play when player moves.
	public AudioClip playerChop2;
	// Use this for initialization
	void Awake ()
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();

	}

	public void DamageWall(int loss)
	{
		SoundManager.instance.RandomizeSfx(this.playerChop1,this.playerChop2);
		spriteRenderer.sprite = dmgSprite;
		hp -= loss;
		if (hp < 0)
			gameObject.SetActive (false);
	}
}

