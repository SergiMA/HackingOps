using HackingOps.Characters.NPC.States;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public abstract class DecisionNode : DecisionTreeNode
    {
        DecisionTreeNode node0;
        DecisionTreeNode node1;

        protected override void InternalAwake()
        {
            base.InternalAwake();

            node0 = transform.GetChild(0).GetComponent<DecisionTreeNode>();
            node1 = transform.GetChild(1).GetComponent<DecisionTreeNode>();
        }

        internal override State Execute()
        {
            if (CheckCondition())
            {
                return node0.Execute();
            }
            else
            {
                return node1.Execute();
            }
        }

        protected abstract bool CheckCondition();
    }
}