using JetBrains.Annotations;
using Lean.Transition.Method;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

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
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = scale;
	}

    public override void PlayRewind(GameObject gameObject)
    {
		PlayForwards(gameObject);
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

	public float RewindSpeed = 1;
	private bool isFeeding = true;
	private ParticleSystem Particles;

	long StartTime;
	long Now;

	ReTime()
	{
        KeyFrames = new List<KeyFrame>();
        StartTime = 0;
        Now = StartTime;
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
		}
		PassDown();
	}

	public void PassDown()
	{
		Debug.Log(RewindSpeed + " < " + gameObject.name);
        //Add the time rewind script to all children - Bubbling
        foreach (Transform child in transform)
        {
            ReTime time = child.gameObject.AddComponent<ReTime>();
            time.RewindSpeed = RewindSpeed;
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

    void FixedUpdate()
    {
        Now += (long)(Time.deltaTime * 1000 * (isRewinding ? -RewindSpeed : 1));

        if (isRewinding)
        {
            Rewind ();
		}else{
			if(isFeeding)
                AddKeyFrame(new TransformKeyFrame(Now, transform));
        }
	}

	//The Rewind method
	void Rewind(){

		KeyFrames.Sort(KeyFrame.rewindSorter);
        while (KeyFrames.Count > 0 && KeyFrames[0].endTimeStamp > Now)
		{
			KeyFrame val = KeyFrames[0];
			val.PlayRewind(gameObject);
			KeyFrames.Remove (val);

        }
	}

	void StartRewind(){

		isRewinding = true;
		if(hasAnimator)
			animator.enabled = false;
	}

	void StopRewind(){
		if (!isRewinding)
		{
			return;
		}
		Time.timeScale = 1;
		isRewinding = false;
		if(hasAnimator)
			animator.enabled = true;
	}

	//exposed method to enable rewind
	public void StartTimeRewind(){
		isRewinding = true;

		if(hasAnimator)
			animator.enabled = false;

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
	}

	//exposed method to disable rewind
	public void StopTimeRewind(){
		isRewinding = false;
		Time.timeScale = 1;
		if(hasAnimator)
			animator.enabled = true;

		if(transform.childCount > 0){
			foreach (Transform child in transform) {

				if (!child.TryGetComponent<ReTime>(out var retime))
				{
					retime = child.AddComponent<ReTime>();
					
				}
                retime.StopTimeRewind ();
			}
		}
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
