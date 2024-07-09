using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace UnityRoyale
{
    public class CardManager : MonoBehaviour
    {
        public Camera mainCamera; //public reference
        public LayerMask playingFieldMask;
        public GameObject cardPrefab;
        // public DeckData playersDeck;
        public MeshRenderer forbiddenAreaRenderer;

        public UnityAction<CardData, Vector3, Faction> OnCardUsed;

        [Header("UI Elements")]
        public RectTransform backupCardTransform; //the smaller card that sits in the deck
        public RectTransform cardsDashboard; //the UI panel that contains the actual playable cards
        public RectTransform cardsPanel; //the UI panel that contains all cards, the deck, and the dashboard (center aligned)

        private Card[] Cards;
        public List<CardData> CardTypes; //list of all available cards
        private bool cardIsActive = false; //when true, a card is being dragged over the play field
        private GameObject previewHolder;
        private Vector3 inputCreationOffset = new Vector3(0f, 0f, 1f); //offsets the creation of units so that they are not under the player's finger
        private HexGridManager hexGridManager;

        private void Awake()
        {
            previewHolder = new GameObject("PreviewHolder");
            hexGridManager = GetComponent<HexGridManager>();

            Cards = CardTypes.Select((x, i) =>
            {
                var card = Instantiate(cardPrefab, cardsPanel);
                card.transform.SetParent(cardsDashboard, true);

                var cardComponent = card.GetComponent<Card>();
                cardComponent.cardId = i;
                cardComponent.OnDragBeginAction += CardDragBegun;
                cardComponent.OnDragAction += CardDragging;
                cardComponent.OnDragEndAction += CardDragEnd;
                cardComponent.InitialiseWithData(x);

                return cardComponent;
            }).ToArray();

            // PlaceCards(Cards);
        }

        Transform ParentAfterDrag;

        private void CardDragBegun(int cardId)
        {
            var card = Cards[cardId];
            ParentAfterDrag = card.transform.parent;
            card.transform.SetParent(card.transform.parent.parent);
            card.transform.SetAsLastSibling();

            forbiddenAreaRenderer.enabled = true;
        }

        private void CardDragging(int cardId, Vector2 dragAmount)
        {
            var card = Cards[cardId];
            // card.portraitImage.raycastTarget = false;
            card.transform.Translate(dragAmount);

            //raycasting to check if the card is on the play field

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _ = Physics.Raycast(ray, out var hit, Mathf.Infinity);

            if (hit.collider?.CompareTag("Grid") ?? false)
            {
                var cell = hexGridManager.GetCell(hit);

                if (!cardIsActive)
                {
                    cardIsActive = true;
                    previewHolder.transform.position = cell.GetCenter();
                    Cards[cardId].ChangeActiveState(true); //hide card
                    hexGridManager.HighlightUnmoveableCells();

                    // retrieve arrays from the CardData
                    UnitType[] dataToSpawn = Cards[cardId].cardData.placeablesData;
                    Vector3[] offsets = Cards[cardId].cardData.relativeOffsets;

                    //spawn all the preview Placeables and parent them to the cardPreview
                    for (int i = 0; i < dataToSpawn.Length; i++)
                    {
                        _ = Instantiate(dataToSpawn[i].Prefab, previewHolder.transform.position, Quaternion.identity, previewHolder.transform);
                    }
                }
                else
                {
                    //temporary copy has been created, we move it along with the cursor
                    previewHolder.transform.position = cell.GetCenter();
                }

                // card.transform.SetParent(hit.transform);

            }
            else
            {
                if (cardIsActive)
                {
                    cardIsActive = false;
                    Cards[cardId].ChangeActiveState(false); //show card

                    ClearPreviewObjects();
                    hexGridManager.BlurUnmoveableCells();
                }
            }
        }

        private void CardDragEnd(int cardId)
        {
            //raycasting to check if the card is on the play field

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _ = Physics.Raycast(ray, out var hit, Mathf.Infinity);

            if (hit.collider?.CompareTag("Grid") ?? false)
            {
                OnCardUsed?.Invoke(Cards[cardId].cardData, hit.point + inputCreationOffset, Faction.Player); //GameManager picks this up to spawn the actual Placeable //GameManager picks this up to spawn the actual Placeable

                ClearPreviewObjects();
                var cell = hexGridManager.GetCell(hit);
                Destroy(Cards[cardId].gameObject); //remove the card itself
                try
                {
                    cell.SetUnit(Cards[cardId].cardData.placeablesData[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                // StartCoroutine(PromoteCardFromDeck(cardId, .2f));
                // StartCoroutine(AddCardToDeck(.6f));
            }
            else
            {
                var card = Cards[cardId];
                card.transform.SetParent(ParentAfterDrag);
                card.transform.SetSiblingIndex(card.cardId);
            }

            hexGridManager.BlurUnmoveableCells();
        }

        //happens when the card is put down on the playing field, and while dragging (when moving out of the play field)
        private void ClearPreviewObjects()
        {
            //destroy all the preview Placeables
            for (int i = 0; i < previewHolder.transform.childCount; i++)
            {
                Destroy(previewHolder.transform.GetChild(i).gameObject);
            }
        }
    }

}
