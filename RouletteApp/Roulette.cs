using System.Security.Cryptography;

public class Roulette
{
	public int CurrentPostion { get; set; }

	public Roulette()
	{
        SpinBall();
	}

	//Pseudo random spin method 

	//public void SpinBall()
	//{ 
	//	Random rng = new Random();
	//	CurrentPostion = rng.Next(0, 37);
	//}

	public void SpinBall()
	{
        var randomBytes = new byte[4];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
            uint trueRandom = BitConverter.ToUInt32(randomBytes, 0);
			CurrentPostion = (int)Math.Abs(trueRandom % 37);
        }
    }
}
