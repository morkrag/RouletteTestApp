using System;
using System.Security.Cryptography;

public class Roulette
{
	public int _currentPostion { get; set; }

	public Roulette()
	{
        SpinBall();
	}

	public void SpinBall()
	{
		Random rng = new Random();
		_currentPostion = rng.Next(0, 37);
	}
}
