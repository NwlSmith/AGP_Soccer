using System;

/*
 * Creator: Jack Schlesinger
 * Date Created: 2/9/2021
 * Description: Behavior Tree base class
 * 
 * Useful for programming different behaviors for AI.
 * 
 * 
 * https://www.youtube.com/watch?v=K6ulNHWMTrY&list=PLE3rHjk-SWif4JAzb4FB48FPJIr3xXZUi&index=3
 * 1:46:41
 */
namespace BehaviorTree
{
    public abstract class Node<T>
    {
        public abstract bool Update(T context);
    }

    public class Tree<T> : Node<T>
    {
        private readonly Node<T> _root;

        public Tree(Node<T> root)
        {
            _root = root;
        }

        public override bool Update(T context)
        {
            return _root.Update(context);
        }
    }

    public class Do<T> : Node<T>
    {
        public delegate bool NodeAction(T context);

        private readonly NodeAction _action;

        public Do(NodeAction action)
        {
            _action = action;
        }

        public override bool Update(T context)
        {
            return _action(context);
        }
    }

    public class Condition<T> : Node<T>
    {
        private readonly Predicate<T> _condition;

        public Condition(Predicate<T> condition)
        {
            _condition = condition;
        }

        public override bool Update(T context)
        {
            return _condition(context);
        }
    }

    public abstract class CompositeNode<T> : Node<T>
    {
        protected Node<T>[] Children { get; private set; }

        protected CompositeNode(params Node<T>[] children)
        {
            Children = children;
        }
    }

    /* Select one child.
     * Go through children from left to right looking for a successful node.
     * If a node fails, it tries the next one.
     * When successful, the node is completed and we can go back up the tree.
     */
    public class Selector<T> : CompositeNode<T>
    {
        public Selector(params Node<T>[] children) : base(children) { }

        public override bool Update(T context)
        {
            foreach (var child in Children)
            {
                if (child.Update(context)) return true;
            }
            return false;
        }
    }

    /* Checklist.
     * Execute from left to right, until a node fails.
     * If the node is successful, do the next one.
     * If the node fails, go back up the tree.
     */
    public class Sequence<T> : CompositeNode<T>
    {
        public Sequence(params Node<T>[] children) : base(children) { }

        public override bool Update(T context)
        {
            foreach (var child in Children)
            {
                if (!child.Update(context)) return false;
            }
            return true;
        }
    }

    public abstract class Decorator<T> : Node<T>
    {
        protected Node<T> Child { get; private set; }

        protected Decorator(Node<T> child)
        {
            Child = child;
        }
    }

    public class Not<T> : Decorator<T>
    {
        public Not(Node<T> child) : base(child) { }

        public override bool Update(T context)
        {
            return !Child.Update(context);
        }
    }
}