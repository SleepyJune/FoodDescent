using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [NonSerialized]
    public Slot slot;

    public Image image;
    public Image background;

    public Text text;

    float slotSize;

    [NonSerialized]
    public bool isSelected = false;
    [NonSerialized]
    public bool isFilled = false;
    [NonSerialized]
    public bool isDead = false;
    [NonSerialized]
    public bool isInteractable = false;


    PathManager pathManager;

    FoodObject nextSpawnFood;

    public Animator anim;

    public void Initialize(Slot slot, float slotSize)
    {
        background.alphaHitTestMinimumThreshold = .05f;

        pathManager = GameManager.instance.pathManager;

        
        this.slotSize = slotSize;

        text.text = "";

        this.slot = slot;

        if (slot != null)
        {
            var worldPos = slot.hexPosition.GetWorldPos(slotSize);
            transform.localPosition = worldPos;

            CreateNewFood(slot.food);
        }

    }

    public void SetFilled(bool filled)
    {
        if (isFilled != filled)
        {
            anim.SetBool("isFilled", filled);
            isFilled = filled;
        }
    }

    public void SetNextFood(FoodObject food)
    {
        nextSpawnFood = food;
    }

    public void CreateNewFood(FoodObject food)
    {
        nextSpawnFood = food;
        CreateNewFood();
    }

    public void CreateNewFood()
    {
        isInteractable = false;

        if(nextSpawnFood != null)
        {
            slot.food = nextSpawnFood;
            nextSpawnFood = null;
        }
        else
        {
            var randomFood = GameManager.instance.foodManager.GenerateRandomFood();
            slot.food = randomFood;
        }

        if (slot.food.isDish)
        {
            anim.SetTrigger("CreateDish");
        }

        image.sprite = slot.food.icon;

        anim.SetTrigger("CreateFood");
    }

    public void SetNewDish()
    {
        anim.SetTrigger("CreateDish");
    }

    public void DestroySlot()
    {
        anim.SetTrigger("DestroyFood");
        isInteractable = false;
    }

    public void SetInteractable()
    {
        if (!isDead)
        {
            isInteractable = true;            
        }
    }

    public void SetDead()
    {
        //isDead = true;

        CreateNewFood();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isInteractable && pathManager)
        {
            pathManager.OnSlotPressed(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isSelected = true;

        if (isInteractable && pathManager)
        {            
            pathManager.OnSlotEnter(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isSelected = false;
    }
}
