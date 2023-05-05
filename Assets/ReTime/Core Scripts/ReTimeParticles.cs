using UnityEngine;

public class ReTimeParticles : MonoBehaviour {
	private ParticleSystem Particles;
	[HideInInspector]
	public float PassedTime;
	private ReTime CoreReTime;
	private float PlaybackTime;
	private float LoopingTime;
	private int PSLoops;
	float t = 0f;
	float MaxTime = 0f;
	bool stop = false;
	bool init = true;
	[Tooltip("ONLY FOR LOOPING PARTICLE SYSTEMS. Enter a float number that will be used as the resetting point after each loop. Will need trial and error and until you reach the perfect float for good visual results")]
	public float ResettingPoint;

	// Use this for initialization
	void Start () {
		Particles = GetComponent<ParticleSystem> ();
		PassedTime = 0.0f;
		LoopingTime = 0.0f;
		PSLoops = 0;
	}
		
	
	// Update is called once per frame
	void Update () {
		if (!CoreReTime)
			CoreReTime = GetComponent<ReTime> ();

		//if the PS is looping, trigger the LOOP TRACKER
		if(Particles.main.loop)
			ParticleSystemLoopTracker ();

		//if stopped emitting and not rewinding, count the delta time
		if (!CoreReTime.isRewinding) {
			init = true;
			//if particle system is dead, start counting
			if (!Particles.IsAlive ())
				PassedTime += Time.deltaTime;

			//if particle system is looping, then count along side
			if (Particles.main.loop)
				LoopingTime += Time.deltaTime;
			
		} else {	
			//if rewinding, minus the passed delta time with current delta time
			PlaybackTime = Particles.time;
			PassedTime -= Time.deltaTime;
			Particles.Stop ();
			init = false;

			//the PS is in loop or not
			if (Particles.main.loop) {
				if (PSLoops > 0) {
					if ( PlaybackTime <= ResettingPoint) {
						PlaybackTime = MaxTime;
						PSLoops--;
					}
				}
				Particles.Simulate (PlaybackTime - Time.deltaTime);
			} else {
				//when the passed time is zero, play the simulation in reverse
				if (PassedTime <= 0.0f)
					Particles.Simulate (PlaybackTime - Time.deltaTime);
			}
		}
	}

	void ParticleSystemLoopTracker(){
		float time = Particles.time;

		if (init) {
			t = Particles.main.duration;

			//if t is bigger than time due to PS time reset and the stop flag is off, then this is the MAX TIME
			if (t - time <= 0.1f) {
				if (!stop) {
					MaxTime = t;
					PSLoops++;
					stop = true;
				}
			} else {
				stop = false;
			}
		}
	}
}
