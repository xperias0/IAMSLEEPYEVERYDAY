using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class MainController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Status
    {
        Normal,
        Climb,
        Destroy
    }
    
    public enum RotDirection
    {
        Forward,
        Backward,
        Left,
        Right
    }

        public Status currentStatus;
        private RotDirection currentRotDirection;
        public Transform forward;
        public Transform backward;
        public Transform left;
        public Transform right;
        public GameObject sleepParticles;

        private Rigidbody rigidbody;
        private GameObject curSleepParticel;
        private GameObject testBall;
        private ParticleSystem parti;
        private Transform[] VertPositions;
        private Transform[] HoriPositons;
        private Queue<Transform> HoriPosQueue;
        private Queue<Transform> VertPosQueue;
        
        private float sleepedTime = 0;
        public float sleepTimePeriod;
        public float rotSpeed;
        public float targetAngle;
        private float stepLength;
        private float vericalAngle    = 0;
        private float horizontalAngle = 0;
        private bool hasDecal  = false;
        private int step       = 0;
        
        private GameObject curDecal;

        private bool isGen = false;
        private bool isOnDecalCube = false;
        private bool isOnWall = false;
        
        private bool startForwardRot  = false;
        private bool startBackwardRot = false;
        private bool startLeftRot     = false;
        private bool startRightRot    = false;
        private bool isFinishedRot    = false;

        private bool isSleeping = true;
        private bool horiDir = false;
        private bool vertDir = false;
        private void Start()
        {
            initialize();
        }

        private void Update()
        {
            rotationControll();
            
            if (!isSleeping)
            {
                sleepedTime += Time.deltaTime;
            }

            if (sleepedTime >= sleepTimePeriod && sleepedTime <sleepTimePeriod+0.1f)
            {
                awakeParicle();
            }
        }

        IEnumerator changeVericalPosition()
        {   
            VertPositions[1] = VertPosQueue.Dequeue();
            VertPositions[0] = VertPosQueue.Peek();
            VertPosQueue.Enqueue(VertPositions[1]);
           
            print("vert Switched");
            //checkPositions();
            return new WaitForSecondsRealtime(0.5f);
        }
        
        IEnumerator changeHorizontalPosition()
        {   
            HoriPositons[1] = HoriPosQueue.Dequeue();
            HoriPositons[0] = HoriPosQueue.Peek();
            HoriPosQueue.Enqueue(HoriPositons[1]);
            
           // print("hori Switched");
            //checkPositions();
            return new WaitForSecondsRealtime(0.5f);
        }
        private void initialize()
        {
            awakeParicle();
            rigidbody = GetComponent<Rigidbody>();
            testBall = GameObject.Find("TESTBALL"); 
            currentStatus = Status.Climb;
            VertPositions = new Transform[2];
            HoriPositons  = new Transform[2];
            HoriPosQueue  = new Queue<Transform>();
            VertPosQueue  = new Queue<Transform>();
            
            VertPositions[0] = left;
            VertPositions[1] = right;
            HoriPositons[0]  = forward;
            HoriPositons[1]  = backward;
            
            HoriPosQueue.Enqueue(forward);
            HoriPosQueue.Enqueue(backward);
            VertPosQueue.Enqueue(left);
            VertPosQueue.Enqueue(right);
        }

        void rotationControll()
        {
            if (startForwardRot)
            {
                horizontalAngle += rotSpeed * Time.deltaTime;
                int dir = vertDir == false ? 1 : -1;
                transform.RotateAround(HoriPositons[0].position,HoriPositons[0].right,(rotSpeed * dir) * Time.deltaTime );
            }

            if (startBackwardRot)
            {
                horizontalAngle += rotSpeed * Time.deltaTime;
                int dir = vertDir == false ? 1 : -1;
                transform.RotateAround(HoriPositons[1].position,HoriPositons[1].right,(-rotSpeed * dir) * Time.deltaTime );
            }
            
            if (startLeftRot)
            {
                vericalAngle += rotSpeed * Time.deltaTime;
                int dir = horiDir == false ? 1 : -1;
                transform.RotateAround(VertPositions[0].position,VertPositions[0].forward,(rotSpeed * dir) * Time.deltaTime );
            }
            
            if (startRightRot)
            {
                vericalAngle += rotSpeed * Time.deltaTime;
                int dir = horiDir == false ? 1 : -1;
                transform.RotateAround(VertPositions[1].position,VertPositions[1].forward,(-rotSpeed * dir) * Time.deltaTime );
            }

            if (vericalAngle >= targetAngle)
            {
                startLeftRot  = false;
                startRightRot = false;
                isFinishedRot = true;
                vertDir = vertDir == false ? true : false;
                print("vertDir: "+vertDir);
                vericalAngle = 0;
                StartCoroutine(changeVericalPosition());
                if(!isOnWall) resetRigidbody();
                
            }

            if (horizontalAngle >= targetAngle)
            {
                startForwardRot  = false;
                startBackwardRot = false;
                isFinishedRot = true;
                horiDir = horiDir == false ? true : false;
//                print("horiDIr: "+horiDir);
                horizontalAngle  = 0;
                StartCoroutine(changeHorizontalPosition());
                if(!isOnWall) resetRigidbody();
                
            }


            if (Input.GetKeyDown(KeyCode.W))  {startForwardRot  = true; OnstartRotation(); currentRotDirection = RotDirection.Forward;}
            if (Input.GetKeyDown(KeyCode.S))  {startBackwardRot = true; OnstartRotation(); currentRotDirection = RotDirection.Backward;}
            if (Input.GetKeyDown(KeyCode.A))  {startLeftRot     = true; OnstartRotation(); currentRotDirection = RotDirection.Left;}
            if (Input.GetKeyDown(KeyCode.D))  {startRightRot    = true; OnstartRotation(); currentRotDirection = RotDirection.Right;}
        }

        private void checkPositions()
        {
            Vector3 vertPos0wld = VertPositions[0].transform.TransformPoint(VertPositions[0].transform.position);
            Vector3 vertPos1wld = VertPositions[1].transform.TransformPoint(VertPositions[1].transform.position);
            Vector3 horiPos0wld =  HoriPositons[0].transform.TransformPoint( HoriPositons[0].transform.position);
            Vector3 horiPos1wld =  HoriPositons[1].transform.TransformPoint( HoriPositons[1].transform.position);
            if (vertPos0wld.x > vertPos1wld.x)
            {
                print("check false : vert");
               
              //  StartCoroutine(changeVericalPosition());
            }

            if (horiPos0wld.z < horiPos1wld.z)
            {
                print("check false :hori");
               
                //StartCoroutine(changeHorizontalPosition());
            }
        }

        void detectCube()
        {

            if (currentStatus == Status.Climb)
            {
                Vector3 targetRotDirection = Vector3.zero;
                Vector3 localTargetRotDirection = Vector3.zero;
                Vector3 startPosition = transform.position + (Vector3.up * 1.3f);
                Vector3 testPos = Vector3.zero;
                float dis = 1.5f;
                float wallAngle = 88f;
                switch (currentRotDirection)
                {
                    case RotDirection.Forward:
                        targetRotDirection      = ((startPosition + Vector3.forward * dis) - startPosition).normalized;
                        localTargetRotDirection = ((startPosition + transform.forward * dis) - startPosition).normalized;
                       
                        break;
                    case RotDirection.Backward:
                            targetRotDirection      = ((startPosition + (-Vector3.forward * dis)) - startPosition).normalized;
                            localTargetRotDirection = ((startPosition + (-transform.forward * dis)) - startPosition).normalized;
                        
                        break;
                    case RotDirection.Left:
                        targetRotDirection = ((startPosition + ((-Vector3.right) * dis)) - startPosition).normalized;
                        break;
                    case RotDirection.Right:
                        targetRotDirection = ((startPosition + (Vector3.right) * dis) - startPosition).normalized;
                        break;
                    
                }

                
                bool isLocalHit = false;
                testBall.transform.position = startPosition;
                
                Ray ray;
                ray = new Ray(startPosition, targetRotDirection);
                Ray localRay;
                localRay = new Ray(startPosition, localTargetRotDirection);
                RaycastHit hit;
                RaycastHit localHit;
                
                if (Physics.Raycast(localRay,out localHit,2.2f, 1 << 6))
                {
                    isLocalHit = true;
                }
                startPosition = !isLocalHit ? startPosition + (-targetRotDirection * 0.2f) : startPosition;
                if (Physics.Raycast(ray, out hit, 1.5f, 1 << 6))
                {
                    rigidbody.useGravity  = false;
                    rigidbody.isKinematic = true;
                    if (isOnWall && isLocalHit )
                    {
                        targetAngle = wallAngle - 5f;
                        if(isOnWall) isOnWall = false;
                        print("localHit");
                    }
                    else
                    {
                        targetAngle = isOnWall ? 180f: wallAngle -1f;
                        isOnWall = true;
                    }
                    
                    print("Cube Detected");
                }
                else
                {
                    targetAngle = isOnWall ? 260f  : 170f;
                    if (isOnWall) isOnWall = false;
                    print("Cube Detected False, isONWALL:" +isOnWall);
                }

                Debug.DrawLine(startPosition, startPosition + targetRotDirection*2f, Color.red, 2f);
            }
        }


        void resetRigidbody()
        {
            
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }

        void OnstartRotation()
        {
            detectCube();
            isFinishedRot = false;
            isSleeping = false;
            sleepedTime = 0;
            Destroy(curSleepParticel);
        }

        void awakeParicle()
        {
            isSleeping = true;
            curSleepParticel = Instantiate(sleepParticles, transform.position, quaternion.identity);
            curSleepParticel.transform.parent = this.transform;
            curSleepParticel.transform.rotation = sleepParticles.transform.rotation;
            sleepedTime = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            float dotResult = Vector3.Dot(transform.up, Vector3.up);
            if (!hasDecal && other.CompareTag("DecalCube") && (dotResult >= 0.95f||dotResult <= -0.95f))
            {
                GameObject decal = other.transform.GetChild(0).gameObject;
                curDecal = Instantiate(decal, transform.position, Quaternion.LookRotation(Vector3.up));
                curDecal.transform.parent = this.transform;
                step = 4;
                stepLength = 1.0f / step;
                hasDecal = true;
            }

            
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("DecalCube"))
            {
                isOnDecalCube = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("DecalCube"))
            {
                isOnDecalCube = false;
            }
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.gameObject.CompareTag("Ground") && step>0)
            {
                
                float dotResult = MathF.Abs(Vector3.Dot(transform.up, Vector3.up));
                if (dotResult <= 0.1)
                {
                    isGen = false;
                }

                bool isGround = dotResult >0.8f;

                
                if (isGround && !isGen && !isOnDecalCube)
                {
                    Vector3 newPos = new Vector3(transform.position.x, -0.4f, transform.position.z);
                    GameObject copy = Instantiate(curDecal, newPos, Quaternion.LookRotation(-Vector3.up));
                    copy.GetComponent<DecalProjector>().fadeFactor     = stepLength * step;
                    curDecal.GetComponent<DecalProjector>().fadeFactor = stepLength * step;
                    isGen = true;
                    step--;
                }

                if (step == 0){
                        Destroy(curDecal);
                        hasDecal = false;
                    }

                
            }
        }


}
