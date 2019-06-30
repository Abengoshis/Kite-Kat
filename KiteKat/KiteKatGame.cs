using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using KiteKat.EcsCore;
using KiteKat.EcsCore.Components;
using KiteKat.Components;
using KiteKat.Systems;

namespace KiteKat
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class KiteKatGame : Game
    {
        // TODO: Replace with configurable scene sizes.
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Random random;

        private EcsWorld world;
        private WindSystem windSystem;

        private Vector2 Gravity = new Vector2(0f, 9.81f);

        public KiteKatGame()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";

            IsFixedTimeStep = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            random = new Random();

            world = new EcsWorld(1000);

            world.RegisterComponentType<Sprite>();
            world.RegisterComponentType<Physics>();
            world.RegisterComponentType<Spring>();
            world.RegisterComponentType<Drift>();
            world.RegisterComponentType<Player>();
            world.RegisterComponentType<Kite>();
            world.RegisterComponentType<Rope>();
            world.RegisterComponentType<CatInterest>();
            world.RegisterComponentType<CatPounce>();
            world.RegisterComponentType<CatSlide>();
            world.RegisterComponentType<CatAbscond>();

           
            world.RegisterUpdateSystem(new PlayerSystem(world));
            world.RegisterUpdateSystem(new KiteSystem(world));
            world.RegisterUpdateSystem(new CatInterestSystem(world));
            world.RegisterUpdateSystem(new CatPounceSystem(world));
            world.RegisterUpdateSystem(new CatSlideSystem(world));
            world.RegisterUpdateSystem(new CatAbscondSystem(world));

            windSystem = new WindSystem(world);
            world.RegisterUpdateSystem(windSystem);

            world.RegisterUpdateSystem(new SpringSystem(world));
            world.RegisterUpdateSystem(new PhysicsSystem(world));
            world.RegisterUpdateSystem(new RopeSystem(world)); // should this just be a render system instead of giving the ropes sprites

            world.RegisterDrawSystem(new SpriteSystem(world, spriteBatch));

            var playerTexture = Content.Load<Texture2D>("Debugging/player");
            var kiteTexture = Content.Load<Texture2D>("Debugging/kite");
            var treeNodeTexture = Content.Load<Texture2D>("Debugging/tree");
            var ropeTexture = Content.Load<Texture2D>("Debugging/rope");
            var catTexture = Content.Load<Texture2D>("Debugging/cat");

            var playerEntity = CreatePlayer(playerTexture);
            var kiteEntity = CreateKite(kiteTexture);
            var ropeEntities = CreateRope(ropeTexture, 32, 200f, world.GetComponent<Transform>(playerEntity).Position, world.GetComponent<Transform>(kiteEntity).Position);
            ConnectPlayerAndKiteWithRope(playerEntity, kiteEntity, ropeEntities);

            var cat1 = CreateCat(catTexture, 200f, 200f);
            var cat2 = CreateCat(catTexture, 300f, 300f);
            var cat3 = CreateCat(catTexture, 800f, 300f);
        }

        // TODO: configurable texture, mass, speed, kite extend/retract speed, 
        private int CreatePlayer(Texture2D playerTexture)
        {
            var playerEntity = world.CreateEntity(512f, 500f);

            var sprite = world.AddComponent<Sprite>(playerEntity);
            sprite.Texture = playerTexture;
            sprite.Origin = playerTexture.Bounds.Center.ToVector2();

            var physics = world.AddComponent<Physics>(playerEntity);
            physics.Mass = 5f;
            physics.Drag = Vector2.UnitX * 7;
            physics.Multiplier = Vector2.UnitX;

            var player = world.AddComponent<Player>(playerEntity);
            player.MoveForce = 240f * physics.Mass;
            player.MoveLeft = Keys.Left;
            player.MoveRight = Keys.Right;

            player.MaxCastDistance = 256f;
            player.CastSpeed = 0.1f;
            player.CastOut = Keys.Up;

            player.MinPullDistance = 128f;
            player.PullSpeed = 0.1f;
            player.PullIn = Keys.Down;

            return playerEntity;
        }

        private int CreateKite(Texture2D kiteTexture)
        {
            var kiteEntity = world.CreateEntity(512f, 300f);

            var sprite = world.AddComponent<Sprite>(kiteEntity);
            sprite.Texture = kiteTexture;
            sprite.Origin = kiteTexture.Bounds.Center.ToVector2();
            sprite.LayerDepth = 0.1f;

            var physics = world.AddComponent<Physics>(kiteEntity);
            physics.Mass = 20f;
            physics.Drag = Vector2.One;

            var drift = world.AddComponent<Drift>(kiteEntity);
            drift.Blowability = Vector2.One * 2;

            var kite = world.AddComponent<Kite>(kiteEntity);

            return kiteEntity;
        }

        private IList<int> CreateRope(Texture2D ropeTexture, int segmentCount, float totalLength, Vector2 startPosition, Vector2 endPosition)
        {
            // TODO: spec
            float stiffness = 30f;
            float damping = 10f;
            float mass = 1f;

            var currentPosition = startPosition;
            var segmentOffset = (endPosition - startPosition) / (segmentCount - 1);
            var segmentLength = totalLength / segmentCount;

            var segments = new int[segmentCount];
            for (int i = 0; i < segmentCount; ++i)
            {
                var segmentEntity = world.CreateEntity(currentPosition);

                var sprite = world.AddComponent<Sprite>(segmentEntity);
                sprite.Texture = ropeTexture;
                sprite.Origin = new Vector2(ropeTexture.Bounds.Right, ropeTexture.Bounds.Center.Y);
                sprite.LayerDepth = 0.8f;

                var rope = world.AddComponent<Rope>(segmentEntity);
                rope.Index = i;

                segments[i] = segmentEntity;
                currentPosition += segmentOffset;
            }

            for (int i = segmentCount - 1; i > 0; --i)
            {
                var currentEntity = segments[i];
                var previousEntity = segments[i - 1];

                // Connect the segment to its previous and next segments.
                var rope = world.GetComponent<Rope>(currentEntity);
                rope.PreviousEntity = previousEntity;

                var previousRope = world.GetComponent<Rope>(previousEntity);
                previousRope.NextEntity = currentEntity;

                // Physically connect the segment to its previous segment with a rigidbody and a spring.
                var physics = world.AddComponent<Physics>(currentEntity);
                physics.Mass = mass;
                physics.Drag = Vector2.One * 0.1f;
                physics.ConstantForce = Gravity * mass;

                var spring = world.AddComponent<Spring>(currentEntity);
                spring.ConnectedEntity = rope.PreviousEntity;
                spring.Stiffness = stiffness;
                spring.Damping = damping;
                spring.RestDistance = segmentLength;
            }

            return segments;
        }

        private void ConnectPlayerAndKiteWithRope(int playerEntity, int kiteEntity, IList<int> ropeEntities)
        {
            // spec
            float stiffness = 1.8f;
            float damping = 50f;

            // Connect the first rope entity to the player.
            var firstRopeEntity = ropeEntities[0];

            var firstRope = world.GetComponent<Rope>(firstRopeEntity);
            firstRope.PreviousEntity = playerEntity;

            var player = world.GetComponent<Player>(playerEntity);
            player.RopeEntity = firstRopeEntity;
            player.KiteEntity = kiteEntity;

            // Connect the last rope entity to the kite.
            var lastRopeEntity = ropeEntities[ropeEntities.Count - 1];

            var lastRope = world.GetComponent<Rope>(lastRopeEntity);
            lastRope.NextEntity = kiteEntity;

            var kite = world.GetComponent<Kite>(kiteEntity);
            kite.RopeEntity = lastRopeEntity;
            kite.PlayerEntity = playerEntity;

            // Connect the kite directly to the player.
            var kiteSpring = world.AddComponent<Spring>(kiteEntity);
            kiteSpring.ConnectedEntity = playerEntity;
            kiteSpring.Stiffness = stiffness;
            kiteSpring.Damping = damping;
            kiteSpring.RestDistance = player.MinPullDistance;
            kiteSpring.RestOrigin = new Vector2(0, 0);
        }

        private int CreateCat(Texture2D catTexture, float x, float y)
        {
            var catEntity = world.CreateEntity(x, y);

            var sprite = world.AddComponent<Sprite>(catEntity);
            sprite.Texture = catTexture;
            sprite.Origin = catTexture.Bounds.Center.ToVector2();

            var catInterest = world.AddComponent<CatInterest>(catEntity);
            catInterest.InterestRate = 1f;
            catInterest.DisinterestRate = 0.5f;

            return catEntity;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float windStrength = 300;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                windSystem.Strength.X = -windStrength;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                windSystem.Strength.X = windStrength;
            }
            else
            {
                windSystem.Strength.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                windSystem.Strength.Y = -windStrength;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                windSystem.Strength.Y = windStrength;
            }
            else
            {
                windSystem.Strength.Y = 0;
            }

            world.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.FrontToBack);
            world.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void Test()
        {
            try
            {
                Console.WriteLine("Test: Register an already registered component type.\n====");
                try
                {
                    world.RegisterComponentType<Physics>();
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                int a = world.CreateEntity();
                world.AddComponent(a, new Transform(new Vector2(413, 413), 0f));
                world.AddComponent(a, new Physics(1f));

                Console.WriteLine("Test: Add a duplicate component.\n====");
                try
                {
                    world.AddComponent(a, new Transform(new Vector2(612, 612), 0f));
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                int b = world.CreateEntity();
                int c = world.CreateEntity();

                Console.WriteLine("Test: Add an entity over the limit.\n====");
                try
                {
                    int d = world.CreateEntity();
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                world.AddComponent(b, new Transform(new Vector2(2, 2), 0f));
                world.AddComponent(c, new Transform(new Vector2(1, 1), 0f));

                Console.WriteLine("Test: Add a component of an unregistered type.\n====");
                try
                {
                    world.AddComponent(b, new Drift(new Vector2(1, 1)));
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                Console.WriteLine("Test: Remove a component of a type not on an object.\n====");
                try
                {
                    world.RemoveComponent<Physics>(b);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                Console.WriteLine("Test: Remove a component that isn't on the object.\n====");
                try
                {
                    world.RemoveComponent(b, new Transform(new Vector2(1, 1), 0f));
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                Transform cTransform = world.RemoveComponent<Transform>(c);

                Console.WriteLine("Test: Remove the same type of component again.\n====");
                try
                {
                    Transform cTransform2 = world.RemoveComponent<Transform>(c);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                Console.WriteLine("Test: Remove the same component again.\n====");
                try
                {
                    Transform cTransform3 = world.RemoveComponent(c, cTransform);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                world.AddComponent(c, new Transform(new Vector2(41, 41), 0f));

                Console.WriteLine("Test: Add the removed component when it has been replaced.\n====");
                try
                {
                    world.AddComponent(c, cTransform);
                }
                catch (Exception e) { Console.WriteLine(e.Message); }

                world.RemoveComponent<Transform>(c);

                world.AddComponent(c, cTransform);
                world.AddComponent(c, new Physics(1f));

                world.RegisterComponentType<Drift>();

                Console.WriteLine("Test: Component can be detected on entity.\n====");
                Console.WriteLine(world.HasComponent<Transform>(a) + " should be true");
                Console.WriteLine(world.HasComponent<Physics>(a) + " should be true");
                Console.WriteLine(world.HasComponent<Transform>(b) + " should be true");
                Console.WriteLine(world.HasComponent<Physics>(b) + " should be false");

                Console.WriteLine("Test: Components can be detected on entity.\n====");
                Console.WriteLine(world.HasComponents(a, typeof(Transform), typeof(Physics)) + " should be true");
                Console.WriteLine(world.HasComponents(a, typeof(Physics), typeof(Transform)) + " should be true");
                Console.WriteLine(world.HasComponents(a, typeof(Physics), typeof(Transform), typeof(Drift)) + " should be false");
                Console.WriteLine(world.HasComponents(b, typeof(Transform)) + " should be true");
                Console.WriteLine(world.HasComponents(b, typeof(Physics)) + " should be false");
                Console.WriteLine(world.HasComponents(b, typeof(Transform), typeof(Physics)) + " should be false");

                Console.WriteLine("Test: Entities can be collected based on component.\n====");
                Console.WriteLine(string.Join(", ", world.GetEntities<Transform>()) + " should be 0, 1, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities<Physics>()) + " should be 0, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities<Drift>()) + " should be empty");

                Console.WriteLine("Test: Entities can be collected based on components.\n====");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Transform))) + " should be 0, 1, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Physics))) + " should be 0, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Transform), typeof(Physics))) + " should be 0, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Physics), typeof(Transform))) + " should be 0, 2");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Drift))) + " should be empty");
                Console.WriteLine(string.Join(", ", world.GetEntities(typeof(Transform), typeof(Physics), typeof(Drift))) + " should be empty");
            }
            catch (Exception e)
            {
                Console.WriteLine("UNCAUGHT EXCEPTION!");
                Console.WriteLine(e.Message);
            }
        }
    }
}
