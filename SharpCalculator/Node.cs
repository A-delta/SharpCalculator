using System;
using System.Collections.Generic;
using System.Text;

namespace SharpCalculator
{
    class Node
    {
        Node m_left;
        String m_value;
        Node m_right;

        public Node(String value)
        {
            this.m_value = value;
            m_left = null;
            m_right = null;
        }

        public Node GetLeftNode()
        {
            return m_left;
        }

        public void SetLeftNode(Node newNode)
        {
            m_left = newNode;
        }



        public Node GetRightNode()
        {
            return m_right;
        }

        public void SetRightNode(Node newNode)
        {
            m_right = newNode;
        }

    }
}


