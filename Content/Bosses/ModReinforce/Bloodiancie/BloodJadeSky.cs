using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Utilities;

namespace Coralite.Content.Bosses.ModReinforce.Bloodiancie
{
    public class BloodJadeSky : CustomSky
    {
        private struct Slime
        {
            public Vector2 Position;
            public float rotation;
            public float Depth;
            public float Speed;
            public bool Active;

            public Texture2D Texture { get; set; }
        }

        private Asset<Texture2D>[] _textures;
        private Slime[] _slimes;
        private UnifiedRandom _random = new();
        private int _slimesRemaining;
        private bool _isActive;
        private bool _isLeaving;
        private int _innerTimer;

        public override void OnLoad()
        {
            _textures = new Asset<Texture2D>[3];
            for (int i = 0; i < 3; i++)
            {
                _textures[i] = ModContent.Request<Texture2D>(AssetDirectory.Bloodiancie + "SkyJade" + (i + 1));
            }

            GenerateSlimes();
        }

        private void GenerateSlimes()
        {
            _slimes = new Slime[Main.maxTilesY / 6];
            for (int i = 0; i < _slimes.Length; i++)
            {
                int num = (int)(Main.screenPosition.Y * 0.7 - Main.screenHeight);
                int minValue = (int)(num - Main.worldSurface * 16.0);
                _slimes[i].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, _random.Next(minValue, num));
                _slimes[i].rotation = Main.rand.NextFloat(6.282f);
                _slimes[i].Speed = 5f + 3f * (float)_random.NextDouble();
                _slimes[i].Depth = (float)i / _slimes.Length * 1.75f + 1.6f;
                _slimes[i].Texture = _textures[_random.Next(3)].Value;
                if (_random.NextBool(60))
                {
                    _slimes[i].Speed = 6f + 3f * (float)_random.NextDouble();
                    _slimes[i].Depth += 0.5f;
                }
                else if (_random.NextBool(30))
                {
                    _slimes[i].Speed = 6f + 2f * (float)_random.NextDouble();
                }

                _slimes[i].Active = true;
            }

            _slimesRemaining = _slimes.Length;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gamePaused || !Main.hasFocus)
                return;

            for (int i = 0; i < _slimes.Length; i++)
            {
                if (!_slimes[i].Active)
                    continue;

                _slimes[i].Position.Y += _slimes[i].Speed;
                _slimes[i].rotation += _slimes[i].Speed / 65;
                if (!(_slimes[i].Position.Y > Main.worldSurface * 16.0))
                    continue;

                if (!_isLeaving)
                {
                    _slimes[i].Depth = i / (float)_slimes.Length * 1.75f + 1.6f;
                    _slimes[i].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, -100f);
                    _slimes[i].rotation = Main.rand.NextFloat(6.282f);
                    _slimes[i].Texture = _textures[_random.Next(2)].Value;
                    _slimes[i].Speed = 5f + 3f * (float)_random.NextDouble();
                    if (_random.NextBool(60))
                    {
                        _slimes[i].Speed = 6f + 3f * (float)_random.NextDouble();
                        _slimes[i].Depth += 0.5f;
                    }
                    else if (_random.NextBool(30))
                    {
                        _slimes[i].Speed = 6f + 2f * (float)_random.NextDouble();
                    }
                }
                else
                {
                    _slimes[i].Active = false;
                    _slimesRemaining--;
                }
            }

            _innerTimer++;
            if (_innerTimer > 60 * 60 * 4)
                _isLeaving = true;

            if (_slimesRemaining == 0)
            {
                _isActive = false;
                _innerTimer = 0;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (Main.screenPosition.Y > 10000f || Main.gameMenu)
                return;

            int num = -1;
            int num2 = 0;
            for (int i = 0; i < _slimes.Length; i++)
            {
                float depth = _slimes[i].Depth;
                if (num == -1 && depth < maxDepth)
                    num = i;

                if (depth <= minDepth)
                    break;

                num2 = i;
            }

            if (num == -1)
                return;

            Vector2 vector = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
            Rectangle rectangle = new(-1000, -1000, 4000, 4000);
            for (int j = num; j < num2; j++)
            {
                if (_slimes[j].Active)
                {
                    Color color = new Color(Main.ColorOfTheSkies.ToVector4() * 0.9f + new Vector4(0.1f)) * 0.8f;
                    float num3 = 1f;
                    if (_slimes[j].Depth > 3f)
                        num3 = 0.6f;
                    else if (_slimes[j].Depth > 2.5)
                        num3 = 0.7f;
                    else if (_slimes[j].Depth > 2f)
                        num3 = 0.8f;
                    else if (_slimes[j].Depth > 1.5)
                        num3 = 0.9f;

                    num3 *= 0.6f;
                    color = new Color((int)(color.R * num3), (int)(color.G * num3), (int)(color.B * num3), (int)(color.A * num3));
                    Vector2 vector2 = new(1f / _slimes[j].Depth, 0.9f / _slimes[j].Depth);
                    Vector2 position = _slimes[j].Position;
                    position = (position - vector) * vector2 + vector - Main.screenPosition;
                    position.X = (position.X + 500f) % 4000f;
                    if (position.X < 0f)
                        position.X += 4000f;

                    position.X -= 500f;
                    if (rectangle.Contains((int)position.X, (int)position.Y))
                    {
                        Vector2 origin = _slimes[j].Texture.Size() / 2;
                        for (int i = 1; i < 8; i += 2)
                            spriteBatch.Draw(_slimes[j].Texture, position - new Vector2(0, i * _slimes[j].Speed), null, color * (0.5f - 0.5f * i / 8), _slimes[j].rotation, origin, vector2.X * 2f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(_slimes[j].Texture, position, null, color, _slimes[j].rotation, origin, vector2.X * 2f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            GenerateSlimes();
            _isActive = true;
            _isLeaving = false;
            _innerTimer = 0;
        }

        public override void Deactivate(params object[] args)
        {
            _isLeaving = true;
        }

        public override void Reset()
        {
            _isActive = false;
            _innerTimer = 0;
        }

        public override bool IsActive() => _isActive;
    }
}
