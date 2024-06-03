using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class TokenInstance : MonoBehaviour
    {
        public AudioClip tokenCollectAudio;
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;
        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation, collectedAnimation;

        internal Sprite[] sprites = new Sprite[0];

        internal SpriteRenderer _renderer;

        //unique index which is assigned by the TokenController in a scene.
        internal int tokenIndex = -1;
        internal TokenController controller;
        //active frame in animation, updated by the controller.
        internal int frame = 0;
        internal bool collected = false;

        public int points = 1;

        [Tooltip("If true, the token will move.")]
        public bool isMoving = false;

        [Tooltip("Speed at which the token moves.")]
        public float speed = 1.0f;

        private Vector3 startPos;

        void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (randomAnimationStartTime)
                frame = Random.Range(0, sprites.Length);
            sprites = idleAnimation;

            // Initialize the start position for movement
            startPos = transform.position;
        }

        void Update()
        {
            if (isMoving)
            {
                // Apply movement logic here
                transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * speed), 0);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            //only execute OnPlayerEnter if the player collides with this token.
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null) OnPlayerEnter(player);
        }

        void OnPlayerEnter(PlayerController player)
        {
            if (collected) return;
            //disable the gameObject and remove it from the controller update list.
            frame = 0;
            sprites = collectedAnimation;
            if (controller != null)
                collected = true;
            //send an event into the gameplay system to perform some behaviour.
            var ev = Schedule<PlayerTokenCollision>();
            ev.token = this;
            ev.player = player;

            // Ajouter des points au score
            ScoreManager.instance.AddScore(points);

            // Détruire l'objet une fois collecté
            Destroy(gameObject);
        }
    }
}
