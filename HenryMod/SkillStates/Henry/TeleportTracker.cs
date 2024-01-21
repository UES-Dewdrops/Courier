using System.Linq;
using RoR2;
using UnityEngine;

namespace HenryMod.SkillStates
{
    public class TeleportTracker : MonoBehaviour
    {
        private const float MaxTrackingDistance = 120f;
        private const float MaxTrackingAngle = 15f;
        private const float TrackerUpdateFrequency = 10f;
        public HurtBox trackingTarget;
        public HurtBox oldtrackingTarget;
        public bool triggerNewTarget;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private Indicator indicator;
        private readonly BullseyeSearch search = new BullseyeSearch();

        private Animator animator;

        private void Awake()
        {
            var visualizerPrefab = Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator");
            indicator = new Indicator(gameObject, visualizerPrefab);
        }

        private void Start()
        {
            inputBank = GetComponent<InputBankTest>();
            teamComponent = GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        private void OnEnable()
        {
            indicator.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
        }

        private void FixedUpdate()
        {
            //System.Console.WriteLine("Fixed Update called!");
            trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (trackerUpdateStopwatch >= 1f / TrackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / TrackerUpdateFrequency;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);

                if (triggerNewTarget)
                {
                    if (animator != null && animator)
                    {
                        animator = Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator").GetComponent<Animator>();
                        animator.Play("NewTarget", -1, 0);
                        triggerNewTarget = false;
                    }
                    else
                    {
                        animator = Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator").GetComponent<Animator>();
                    }
                }
            }
        }
        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.GetUnprotectedTeams(teamComponent.teamIndex);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = MaxTrackingDistance;
            search.maxAngleFilter = MaxTrackingAngle;
            search.RefreshCandidates();
            trackingTarget = search.GetResults().FirstOrDefault();

            if (trackingTarget != null && trackingTarget)
            {
                if (oldtrackingTarget != null)
                {
                    if (oldtrackingTarget)
                    {
                        if (oldtrackingTarget != trackingTarget)
                        {
                            triggerNewTarget = true;
                            oldtrackingTarget = trackingTarget;
                        }
                        else
                        {
                            triggerNewTarget = false;
                        }
                    }
                    else
                    {
                        triggerNewTarget = true;
                        oldtrackingTarget = trackingTarget;
                    }
                }
                else
                {
                    triggerNewTarget = true;
                    oldtrackingTarget = trackingTarget;
                }
            }
        }

    }
}