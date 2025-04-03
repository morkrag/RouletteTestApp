using System;

public class Number
{
	public int _value {  get; }
	public string _color { get; set; }

	public Number(int value)
	{
		if (value < 0 || value > 36)
		{
			throw new ArgumentOutOfRangeException(nameof(value), $"Roulette numbers are from 0 to 36, {value} is out of range"); 
		}
		_value = value;
		setColor();
	}

	private void setColor()
	{
		if (_value == 0)
		{
			_color = "green";
		} else if (_value > 0 && _value < 11 || _value > 18 && _value < 29)
			{
				if (_value % 2 == 1)
				{
					_color = "black";
				}
				else
				{
					_color = "red";
				}
		} else
			{
				if (_value % 2 == 1)
				{
					_color = "black";
				}
				else
				{
					_color = "red";
				}
			}
	}
}
