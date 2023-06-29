using JetBrains.Annotations;
using Lean.Transition.Method;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Xml;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
	
public class KeyFrameForwardSorter : IComparer<KeyFrame>
{
    public int Compare(KeyFrame c1, KeyFrame c2)
    {
        return c2.endTimeStamp.CompareTo(c1.startTimeStamp);
    }
}

public class KeyFrameRewindSorter : IComparer<KeyFrame>
{
    public int Compare(KeyFrame c1, KeyFrame c2)
    {
        return c2.endTimeStamp.CompareTo(c1.endTimeStamp);
    }
}


public abstract class KeyFrame {

    public static KeyFrameForwardSorter forwardsSorter = new();
    public static KeyFrameRewindSorter rewindSorter = new();

    public long startTimeStamp;
	public int durationMilliSeconds;
	public long endTimeStamp;

	public KeyFrame(long startTimeStamp) : this(startTimeStamp, 0)
	{

	}

    public KeyFrame(long startTimeStamp, int duration)
    {
		this.startTimeStamp = startTimeStamp;
		this.durationMilliSeconds = duration;
		this.endTimeStamp = startTimeStamp + duration;
    }

    public abstract void PlayForwards(GameObject gameObject);
    public abstract void PlayRewind(GameObject gameObject);
}

public class RunnableKeyFrame : KeyFrame
{
	private Action<GameObject> forwards;
	private Action<GameObject> backwards;

	public RunnableKeyFrame(long now, int duration, Action<GameObject> forwards, Action<GameObject> backwards)
		: base(now, duration)
	{
		this.forwards = forwards;
		this.backwards = backwards;
	}

	public RunnableKeyFrame(long now, int duration, Action<GameObject> run)
		: this(now, duration, run, run)
	{
	}

    public override void PlayForwards(GameObject gameObject)
    {
		this.forwards(gameObject);
    }
    public override void PlayRewind(GameObject gameObject)
    {
        this.backwards(gameObject);
    }
}

public class TransformKeyFrame : KeyFrame {

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 scale;

	public TransformKeyFrame(long startTimeStamp, Transform transform) : base(startTimeStamp, 0)
	{
	    this.position = transform.position;
        this.rotation = transform.rotation;
		this.scale = transform.localScale;
	}

	public override void PlayForwards(GameObject gameObject)
	{
        if (gameObject.TryGetComponent<Rigidbody2D>(out var rb) && rb.bodyType != RigidbodyType2D.Static)
        {
            rb.MovePosition(position);
        }
        else
        {
			gameObject.transform.position = position;
        }
        gameObject.transform.rotation = rotation;
		gameObject.transform.localScale = scale;
	}

    public override void PlayRewind(GameObject gameObject)
    {
		PlayForwards(gameObject);
    }
}

public class AnimKeyFrame : KeyFrame
{
	public AnimationClip clip;
	public float timeScale;

    public AnimKeyFrame(long now, int length, AnimationClip clip, float timeScale) : base(now, length)
    {
        this.clip = clip;
		this.timeScale = timeScale;
    }

	public override void PlayForwards(GameObject gameObject)
	{
		Play(gameObject, 1);
	}
    public override void PlayRewind(GameObject gameObject)
	{
		Play(gameObject, -timeScale);
	}
    void Play(GameObject gameObject, float ts)
	{
		if (!gameObject.TryGetComponent<GraphHolder>(out var graphHolder)) return;
		PlayableGraph graph = graphHolder.graph;
        var playableOutput = AnimationPlayableOutput.Create(graph, "Animation", gameObject.GetComponent<Animator>());
        var playableClip = AnimationClipPlayable.Create(graph, clip);
		playableClip.SetSpeed(ts);
		playableOutput.SetSourcePlayable(playableClip);
    }
}

public class AudioKeyFrame : KeyFrame {

	public AudioSource audioSource;
	private float pitch = 1;

	public AudioKeyFrame(long now, AudioSource audioSource) : base(now, (int) (audioSource.clip.length * 1000)) {
		this.audioSource = audioSource;
	}

    public override void PlayForwards(GameObject gameObject)
    {
        throw new NotImplementedException();
    }

    public override void PlayRewind(GameObject gameObject)
    {
		pitch = audioSource.pitch;
		audioSource.pitch = -1;
		audioSource.loop = true;
		audioSource.time = 0;
		audioSource.Play();
        gameObject.GetComponent<ReTime>().StartCoroutine(cleanup());
	}

    public IEnumerator cleanup()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.loop = false;
		audioSource.pitch = pitch;
    }
}

public class ReTime : MonoBehaviour {
	//flagging to enable or disable rewind
	[HideInInspector]
	public bool isRewinding = false;

	//use a linked list data structure for better performance of accessing previous positions and rotations;
	private List<KeyFrame> KeyFrames;

	private bool hasAnimator = false;
    [HideInInspector]
    public Animator animator;
	// Set to true will create a zombie like clone that plays forwards once stopping to rewind
	[HideInInspector]
	public bool IsNewTimeline = false;

	public float RewindSpeed = 1;
	private bool isFeeding = true;
	private ParticleSystem Particles;

	long Now;
    long CurrentAnimationStart;

    ReTime()
	{
        KeyFrames = new List<KeyFrame>();
        Now = 0;
		CurrentAnimationStart = 0;
    }

    // Use this for initialization
    void Start () {

        //if contains particle system, then cache and add comp.
        if (GetComponent<ParticleSystem> ())
			Particles = GetComponent<ParticleSystem> ();
		
		//cache if has animator
		if (GetComponent<Animator> ()) {
			hasAnimator = true;
			animator = GetComponent<Animator> ();
            gameObject.AddComponent<GraphHolder>();
        }

		PassDown();
	}

    public void PassDown()
	{
        //Add the time rewind script to all children - Bubbling
        foreach (Transform child in transform)
        {
			if (!child.gameObject.TryGetComponent<ReTime>(out var _))
			{
                ReTime time = child.gameObject.AddComponent<ReTime>();
                time.RewindSpeed = RewindSpeed;
            }
        }
    }

	public void AddKeyFrame (KeyFrame frame)
	{
		KeyFrames.Add(frame);
    }

    public void AddKeyFrame(Action<GameObject> run)
    {
        AddKeyFrame(new RunnableKeyFrame(Now, 0, run));
		run(gameObject);
    }

    public void AddKeyFrame(int duration, Action<GameObject> run)
    {
        AddKeyFrame(new RunnableKeyFrame(Now, duration, run));
        run(gameObject);
    }

    public void AddKeyFrame(Action<GameObject> forwards, Action<GameObject> backwards)
    {
        AddKeyFrame(new RunnableKeyFrame(Now, 0, forwards, backwards));
        forwards(gameObject);
    }

    public void AddKeyFrame(int duration, Action<GameObject> forwards, Action<GameObject> backwards)
    {
        AddKeyFrame(new RunnableKeyFrame(Now, duration, forwards, backwards));
        forwards(gameObject);
    }

    public void AddKeyFrameAudio(AudioSource audioSource)
	{
		AddKeyFrame(new AudioKeyFrame(Now, audioSource));
    }

	private AnimationClip lastAnim = null;

    void FixedUpdate()
    {
        Now += (long)(Time.deltaTime * 1000 * (isRewinding ? -RewindSpeed : 1));

		if(!isRewinding && isFeeding)
		{
            AddKeyFrame(new TransformKeyFrame(Now, transform));
			if (hasAnimator)
			{
				CreateAnimKeyFrame(false);
            }
        }
    }

	private void CreateAnimKeyFrame(bool forceSave)
	{
        AnimationClip cur = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        if (forceSave || lastAnim == null || cur != null && cur.name != lastAnim.name)
        {
            if (lastAnim != null)
                AddKeyFrame(new AnimKeyFrame(CurrentAnimationStart, (int)(Now - CurrentAnimationStart), lastAnim, RewindSpeed));
            lastAnim = cur;
            CurrentAnimationStart = Now;
        }
    }

	//exposed method to enable rewind
	public void StartTimeRewind(){
		isRewinding = true;

		if (hasAnimator)
        {
            CreateAnimKeyFrame(true);
            if (TryGetComponent<GraphHolder>(out var graph))
			{
				graph.graph.Play();
			}
        }

		if(transform.childCount > 0){
			foreach (Transform child in transform)
			{
                if (!child.TryGetComponent<ReTime>(out var retime))
                {
                    retime = child.AddComponent<ReTime>();
                }
                retime.StartTimeRewind();
            }
		}

		if (!TryGetComponent<KeyFramePlayer>(out var player))
		{
			player = gameObject.AddComponent<KeyFramePlayer>();
		}
		player.Play(KeyFrames, Now);
        player.SetTimeScale(() => isRewinding ? -RewindSpeed : 1);
    }

	//exposed method to disable rewind
	public void StopTimeRewind(){
		
        if (transform.childCount > 0){
			foreach (Transform child in transform) {

				if (!child.TryGetComponent<ReTime>(out var retime))
				{
					retime = child.AddComponent<ReTime>();
					
				}
                retime.StopTimeRewind ();
			}
        }

		if (isRewinding && gameObject.CompareTag("Player"))
		{
			GameObject clone = Instantiate(gameObject);
            Destroy(clone.GetComponent<ReTime>());
            Destroy(clone.GetComponent<PlayerInput>());
            clone.transform.position = transform.position;
			if (!clone.TryGetComponent<KeyFramePlayer>(out var player))
			{
                player = clone.AddComponent<KeyFramePlayer>();

            }
			if (clone.TryGetComponent<Rigidbody2D>(out var rb))
			{
				rb.bodyType = RigidbodyType2D.Kinematic;
			}
			player.Stop();
			long time = KeyFrames.FindLast(k => true).endTimeStamp;
            var KeyFramesMod = new List<KeyFrame>(KeyFrames)
            {
                new RunnableKeyFrame(time, 0, g => {
					GameObject clone = new();
					clone.transform.position = g.transform.position;
                    SoundPlayer.current.PlaySound(Sound.PLAYER_TIMELINE_END, clone.transform);
                    Destroy(g);
				})
            };
            player.Play(KeyFramesMod, Now);
			player.SetTimeScale(() => isRewinding ? -RewindSpeed : 1);
			if (clone.TryGetComponent<SpriteRenderer>(out var sprite))
			{
				sprite.color = new Color(95/255f, 146/255f, 1);
			}
            if (clone.TryGetComponent<GraphHolder>(out var cloneGraph))
            {
                cloneGraph.graph.Play();
            }
        }

        if (hasAnimator && TryGetComponent<GraphHolder>(out var graph))
		{
            graph.graph.Stop();
        }

        if (TryGetComponent<KeyFramePlayer>(out var p)) { 
			p.Stop();
        }
		KeyFrames.Clear();
        isRewinding = false;
    }

    //Check point end for parent obect
    public void StopFeeding(){
		isFeeding = false;

		if(transform.childCount > 0) {
			foreach (Transform child in transform) {
				child.GetComponent<ReTime>().StopFeeding ();
			}
		}
	}

	//Check point start for parent obect
	public void StartFeeding(){
		isFeeding = true;

		if(transform.childCount > 0){
			foreach (Transform child in transform) {
				child.GetComponent<ReTime>().StartFeeding ();
			}
		}
	}

	//on adding ReTime component, also add the particles script to all objects that are PS
	void Reset(){
		if(GetComponent<ParticleSystem>())
			gameObject.AddComponent<ReTimeParticles> ();
		
		//Add the particles script to all children that are PS - Bubbling
		foreach (Transform child in transform) {
			if(child.GetComponent<ParticleSystem>())
				child.gameObject.AddComponent<ReTimeParticles> ();
		}
	}
}
