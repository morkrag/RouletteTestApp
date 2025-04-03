public class Number
{
	public int Value {  get; }
	public string Color { get; set; }

	public Number(int value)
	{
		if (value < 0 || value > 36)
		{
			throw new ArgumentOutOfRangeException(nameof(value), $"Roulette numbers are from 0 to 36, {value} is out of range"); 
		}
		Value = value;
		setColor();
	}

	private void setColor()
	{
		if (Value == 0)
		{
			Color = "green";
		} else if (Value > 0 && Value < 11 || Value > 18 && Value < 29)
			{
				if (Value % 2 == 1)
				{
					Color = "red";
				}
				else
				{
					Color = "black";
				}
		} else
			{
				if (Value % 2 == 1)
				{
					Color = "black";
				}
				else
				{
					Color = "red";
				}
			}
	}
}
