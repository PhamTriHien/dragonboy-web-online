/// <summary>
/// INTERFACE IMapObject: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public interface IMapObject
{
	int getX();

	int getY();

	int getW();

	int getH();

	void stopMoving();

	bool isInvisible();
}
