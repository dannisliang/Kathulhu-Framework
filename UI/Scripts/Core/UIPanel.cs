namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;


    public class UIPanel : UIBehaviour
    {

        const string STATE_ACTIVE = "ACTIVE";
        const string STATE_INACTIVE = "INACTIVE";
        const string PARAM_ACTIVATE = "Activate";

        private int _activateParamID;

        private Animator _animator;

        protected override void Awake()
        {
            base.Awake();

            _activateParamID = Animator.StringToHash( PARAM_ACTIVATE );

            _animator = GetComponent<Animator>();
        }


        /// <summary>
        /// Activates the panel and triggers the activation transition 
        /// </summary>
        public void Activate()
        {
            gameObject.SetActive( true );

            if ( _animator != null )
            {
                _animator.SetBool( _activateParamID, true );
            }

            OnActivate();
        }


        /// <summary>
        /// Override this method to add activation logic to the panel
        /// </summary>
        protected virtual void OnActivate()
        {
            //
        }

        /// <summary>
        /// Deactivates the panel and triggers the deactivation transition
        /// </summary>
        public void Deactivate()
        {
            if ( gameObject.activeInHierarchy )
            {
                if ( _animator != null )
                {
                    _animator.SetBool( _activateParamID, false );
                }

                StartCoroutine( Deactivation() );

                OnDeactivate();
            }
        }

        /// <summary>
        /// Override this method to add deactivation logic to the panel
        /// </summary>
        protected virtual void OnDeactivate()
        {
            //
        }


        private IEnumerator Deactivation()
        {
            bool wantToClose = true;

            if ( _animator != null )
            {
                bool closedStateReached = false;
                while ( !closedStateReached && wantToClose )
                {
                    if ( !_animator.IsInTransition( 0 ) )
                        closedStateReached = _animator.GetCurrentAnimatorStateInfo( 0 ).IsName( STATE_INACTIVE );

                    wantToClose = !_animator.GetBool( _activateParamID );
                    yield return new WaitForEndOfFrame();
                }
            }

            if ( wantToClose )
                gameObject.SetActive( false );
        }

    }
}
