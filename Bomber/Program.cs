using System;
using SplashKitSDK;
using System.Collections.Generic;

namespace BomberGame
{
    public class Program
    {
        public static void Main()
        {
            Window gameWindow = new Window("Bomber Game", 750, 750);
            Bomber game = new Bomber(gameWindow);

            while (!game.Quit && !gameWindow.CloseRequested)
            {
                SplashKit.ProcessEvents();
                game.HandleInput();
                game.Update();
                game.Draw();
            }

            gameWindow.Close();
        }
    }

    public class Bomber
    {
        private Player _player;
        private Window _gameWindow;
        private List<Monster> _monsters;
        private List<Bomb> _bombs;
        private List<Wall> _walls;
        private bool _quit;
        private bool _gameOver = false;

        public bool Quit { get { return _quit; } }
        public bool GameOver { get { return _gameOver; } }

        public Bomber(Window gameWindow)
        {
            _gameWindow = gameWindow;
            _player = new Player(gameWindow);
            _monsters = new List<Monster>();
            _bombs = new List<Bomb>();
            _walls = new List<Wall>();
            _quit = false;

            GenerateWalls();
            GenerateMonsters();
        }

        private void GenerateWalls()
        {
            for (int i = 50; i <= 700; i += 100)
            {
                for (int j = 50; j <= 700; j += 100)
                {
                    _walls.Add(new UnbreakableWall(i, j));
                }
            }
            for (int i = 0; i <= 750; i += 50)
            {
                for (int j = 0; j <= 750; j += 100)
                {
                    if (!(i == 0 && j == 0) &&
                        !(i == 0 && j == 700) &&
                        !(i == 50 && j == 0) &&
                        !(i == 50 && j == 700) &&
                        !(i == 100 && j == 700) &&
                        !(i == 150 && j == 700) &&
                        !(i == 200 && j == 300) &&
                        !(i == 250 && j == 300) &&
                        !(i == 300 && j == 300) &&
                        !(i == 350 && j == 300) &&
                        !(i == 600 && j == 600) &&
                        !(i == 600 && j == 700) &&
                        !(i == 700 && j == 0) &&
                        !(i == 700 && j == 100))
                    {
                        _walls.Add(new BreakableWall(i, j));
                    }
                }
            }
            for (int i = 0; i <= 750; i += 100)
            {
                for (int j = 50; j <= 750; j += 100)
                {
                    if (!(i == 0 && j == 50) &&
                        !(i == 600 && j == 550) &&
                        !(i == 600 && j == 650) &&
                        !(i == 700 && j == 50) &&
                        !(i == 700 && j == 200))
                    {
                        _walls.Add(new BreakableWall(i, j));
                    }
                }
            }
        }

        private void GenerateMonsters()
        {
            _monsters.Add(new VerticalMonster(600, 550));
            _monsters.Add(new VerticalMonster(700, 0));
            _monsters.Add(new HorizontalMonster(0, 700));
            _monsters.Add(new HorizontalMonster(200, 300));
        }

        public void HandleInput()
        {
            _player.HandleInput(_bombs);
        }

        public void Update()
        {
            _player.StayOnWindow(_gameWindow);
            _player.StayOnRoad(_walls);

            foreach (Monster monster in _monsters)
            {
                monster.Update();
                monster.StayOnWindow(_gameWindow);
                monster.StayOnRoad(_walls);
                monster.HandleBombCollision(_bombs);
            }

            for (int i = _bombs.Count - 1; i >= 0; i--)
            {
                _bombs[i].Update(_player, _monsters, _walls);
                if (_bombs[i].Exploded)
                {
                    _bombs.RemoveAt(i);
                }
            }

            CheckCollisions();
        }

        private void CheckCollisions()
        {
            foreach (Monster monster in _monsters)
            {
                if (_player.CollidedWith(monster))
                {
                    _gameOver = true;
                    break;
                }
            }

            if (_player.InBombRange) 
            {
                _gameOver = true;
            }
        }

        public void Draw()
        {
            _gameWindow.Clear(Color.White);

            foreach (Wall wall in _walls)
            {
                wall.Draw();
            }

            _player.Draw();

            foreach (Monster monster in _monsters)
            {
                monster.Draw();
            }

            foreach (Bomb bomb in _bombs)
            {
                bomb.Draw();
            }

            if (_gameOver)
            {
                _gameWindow.Clear(Color.Black);
                SplashKit.DrawText("GAME OVER!", Color.White, 325, 325);
            }

            if (_monsters.Count == 0)
            {
                _gameWindow.Clear(Color.Blue);
                SplashKit.DrawText("VICTORY!", Color.White, 325, 325);
            }

            _gameWindow.Refresh(60);
        }
    }

    public class Player
    {
        private double _x;
        private double _y;
        private int step;
        private bool _quit;
        private Bitmap _playerBitmap;
        private bool _inBombRange = false;
        private bool _bombPlanted = false;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Width
        {
            get
            {
                return _playerBitmap.Width;
            }
        }

        public int Height
        {
            get
            {
                return _playerBitmap.Height;
            }

        }

        public int Speed
        {
            get
            {
                return 5;
            }
        }

        public bool Quit { get { return _quit; } }
        public bool InBombRange { get { return _inBombRange; } set { _inBombRange = value; } }
        public bool BombPlanted { get { return _bombPlanted; } set { _bombPlanted = value; } }

        public Player(Window gameWindow)
        {
            _playerBitmap = new Bitmap("Player", "Player.jpg");
            _x = 0;
            _y = 0;
            _quit = false;
        }

        public void HandleInput(List<Bomb> bombs)
        {

            if (SplashKit.KeyDown(KeyCode.UpKey) || (SplashKit.KeyDown(KeyCode.WKey)))
            {
                Y -= Speed;
            }
            else if (SplashKit.KeyDown(KeyCode.DownKey) || (SplashKit.KeyDown(KeyCode.SKey)))
            {
                Y += Speed;
            }
            else if (SplashKit.KeyDown(KeyCode.LeftKey) || (SplashKit.KeyDown(KeyCode.AKey)))
            {
                X -= Speed;
            }
            else if (SplashKit.KeyDown(KeyCode.RightKey) || (SplashKit.KeyDown(KeyCode.DKey)))
            {
                X += Speed;
            }
            if (SplashKit.KeyDown(KeyCode.EscapeKey))
            {
                _quit = true;
            }
            if (SplashKit.KeyTyped(KeyCode.SpaceKey) && !_bombPlanted) PlantBomb(bombs);
        }

        private void PlantBomb(List<Bomb> bombs)
        {
            bombs.Add(new Bomb(_x, _y));
            _bombPlanted = true;
        }

        public void StayOnWindow(Window limit)
        {
            const int GAP = 0;

            if (X < GAP)
            {
                X = GAP;
            }
            if (_playerBitmap.Width + X > limit.Width - GAP)
            {
                X = limit.Width - GAP - _playerBitmap.Width;
            }
            if (Y < GAP)
            {
                Y = GAP;
            }
            if (_playerBitmap.Height + Y > limit.Height - GAP)
            {
                Y = limit.Height - GAP - _playerBitmap.Height;
            }
        }

        public void StayOnRoad(List<Wall> walls)
        {
            foreach (Wall wall in walls)
            {
                if (SplashKit.BitmapCollision(_playerBitmap, X, Y, wall.WallBitmap, wall.X, wall.Y))
                {
                    if (SplashKit.KeyDown(KeyCode.LeftKey) || (SplashKit.KeyDown(KeyCode.AKey))) 
                    {
                        X += Speed;
                    }
                    if (SplashKit.KeyDown(KeyCode.RightKey) || (SplashKit.KeyDown(KeyCode.DKey))) 
                    {
                        X -= Speed;
                    }
                    if (SplashKit.KeyDown(KeyCode.UpKey) || (SplashKit.KeyDown(KeyCode.WKey))) 
                    {
                        Y += Speed;
                    }
                    if (SplashKit.KeyDown(KeyCode.DownKey) || (SplashKit.KeyDown(KeyCode.SKey))) 
                    {
                        Y -= Speed;
                    }
                }
            }
        }

        public bool CollidedWith(Monster other)
        {
            return _playerBitmap.CircleCollision(X, Y, other.CollisionCircle);
        }


        public void Draw()
        {
            _playerBitmap.Draw(_x, _y);
        }
    }

    public abstract class Wall
    {
        protected double _x;
        protected double _y;
        protected Bitmap _wallBitmap;

        public double X { get { return _x; } }
        public double Y { get { return _y; } }
        public Bitmap WallBitmap { get { return _wallBitmap; } }

        protected int Width
        {
            get
            {
                return _wallBitmap.Width;
            }
        }

        protected int Height
        {
            get
            {
                return _wallBitmap.Height;
            }

        }
        public abstract bool IsBreakable { get; }

        public Wall(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public void Draw()
        {
            _wallBitmap.Draw(_x, _y);
        }
    }

    public class UnbreakableWall : Wall
    {
        public UnbreakableWall(double x, double y) : base(x, y)
        {
            _wallBitmap = new Bitmap("UnbreakableWall", "UnbreakableWall.jpg");
        }
        public override bool IsBreakable { get { return false; } }
    }

    public class BreakableWall : Wall
    {
        public BreakableWall(double x, double y) : base(x, y)
        {
            _wallBitmap = new Bitmap("BreakableWall", "BreakableWall.jpg");
        }
        public override bool IsBreakable { get { return true; } }
    }

    public abstract class Monster
    {
        protected double _x;
        protected double _y;
        protected int _speed = 1;
        protected Bitmap _monsterBitmap;

        public double X { get { return _x; } }
        public double Y { get { return _y; } }
        public Bitmap MonsterBitmap { get { return _monsterBitmap; } }
        public abstract void Update();
        public abstract void StayOnWindow(Window gameWindow);
        public abstract void StayOnRoad(List<Wall> walls);
        public abstract void HandleBombCollision(List<Bomb> bombs);

        public int Width
        {
            get
            {
                return 50;
            }
        }
        public int Height
        {
            get
            {
                return 50;
            }
        }

        public Circle CollisionCircle
        {
            get
            {
                return SplashKit.CircleAt(X + Width / 2, Y + Height / 2, 20);
            }
        }

        public void Draw()
        {
            _monsterBitmap.Draw(_x, _y);
        }
    }

    public class VerticalMonster : Monster
    {
        private bool _movingDown = true;

        public VerticalMonster(double x, double y)
        {
            _monsterBitmap = new Bitmap("VerticalMonster", "VerticalMonster.png");
            _x = x;
            _y = y;
        }

        public override void Update()
        {
            if (_movingDown) _y += _speed;
            else _y -= _speed;
        }

        public override void StayOnWindow(Window gameWindow)
        {
            const int GAP = 0;
            if (_y < GAP) {
                _y = GAP;
                _movingDown = true;
            }
            if (_y + Height > gameWindow.Height - GAP) {
                _y = gameWindow.Height - GAP - Height;
                _movingDown = false;
            }
        }

        public override void StayOnRoad(List<Wall> walls)
        {
            foreach (Wall wall in walls)
            {
                if (SplashKit.BitmapCollision(_monsterBitmap, X, Y, wall.WallBitmap, wall.X, wall.Y))
                {
                    _movingDown = !_movingDown;
                    return; // Exit after the first collision to avoid multiple reversals in one frame
                }
            }
        }

        public override void HandleBombCollision(List<Bomb> bombs)
        {
            foreach (Bomb bomb in bombs)
            {
                if (bomb.CollidesWithMonster(this))
                {
                    _movingDown = !_movingDown;
                    return; // Exit after the first collision to avoid multiple reversals in one frame
                }
            }
        }
    }

    public class HorizontalMonster : Monster
    {
        private bool _movingRight = true;

        public HorizontalMonster(double x, double y)
        {
            _monsterBitmap = new Bitmap("HorizontalMonster", "HorizontalMonster.png");
            _x = x;
            _y = y;
        }

        public override void Update()
        {
            if (_movingRight) _x += _speed;
            else _x -= _speed;
        }

        public override void StayOnWindow(Window gameWindow)
        {
            const int GAP = 0;
            if (_x < 0) {
                _x = GAP;
                _movingRight = true;
            }
            if (_x + Width > gameWindow.Width) {
                _x = gameWindow.Width - GAP - Width;
                _movingRight = false;
            }
        }

        public override void StayOnRoad(List<Wall> walls)
        {
            foreach (Wall wall in walls)
            {
                if (SplashKit.BitmapCollision(_monsterBitmap, X, Y, wall.WallBitmap, wall.X, wall.Y))
                {
                    _movingRight = !_movingRight;
                    return; // Exit after the first collision to avoid multiple reversals in one frame
                }
            }
        }

        public override void HandleBombCollision(List<Bomb> bombs)
        {
            foreach (Bomb bomb in bombs)
            {
                if (bomb.CollidesWithMonster(this))
                {
                    _movingRight = !_movingRight;
                    return; // Exit after the first collision to avoid multiple reversals in one frame
                }
            }
        }
    }

    public class Bomb
    {
        private double _x;
        private double _y;
        private int _width = 30;
        private int _height = 30;
        private Bitmap _bombBitmap;
        private SplashKitSDK.Timer _timer;
        private bool _exploded;

        public double X { get { return _x; } }
        public double Y { get { return _y; } }
        public bool Exploded { get { return _exploded; } }

        public Bomb(double x, double y)
        {
            _bombBitmap = new Bitmap("Bomb", "Bomb.jpg");
            _x = x;
            _y = y;
            _timer = new SplashKitSDK.Timer("BombTimer");
            _timer.Start();
            _exploded = false;
        }

        public void Update(Player player, List<Monster> monsters, List<Wall> walls)
        {
            if (!_exploded && _timer.Ticks > 2000)
            {
                Explode(player, monsters, walls);
                _exploded = true;
                player.BombPlanted = false;
            }
        }

        private void Explode(Player player, List<Monster> monsters, List<Wall> walls)
        {
            // Check for monsters within explosion range
            for (int i = monsters.Count - 1; i >= 0; i--)
            {
                if (InExplosionRange(monsters[i].X, monsters[i].Y))
                {
                    monsters.RemoveAt(i);
                }
            }

            // Check for breakable walls within explosion range
            for (int i = walls.Count - 1; i >= 0; i--)
            {
                if (walls[i] is BreakableWall && InExplosionRange(walls[i].X, walls[i].Y))
                {
                    walls.RemoveAt(i);
                }
            }

            if (InExplosionRange(player.X, player.Y))
            {
                player.InBombRange = true;
            }
        }

        public bool InExplosionRange(double otherX, double otherY)
        {
            return (Math.Abs(_x - otherX) <= _width + 50 && _y == otherY) ||
           (Math.Abs(_y - otherY) <= _height + 50 && _x == otherX);
        }

        public bool CollidesWithMonster(Monster monster)
        {
            return SplashKit.BitmapCollision(_bombBitmap, _x, _y, monster.MonsterBitmap, monster.X, monster.Y);
        }

        public void Draw()
        {
            if (!_exploded)
            {
                _bombBitmap.Draw(_x, _y);
            }
        }
    }
}

