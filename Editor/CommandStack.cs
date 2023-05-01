using System.Collections.Generic;
using UnityEngine;

namespace Editor
{
    public class Command
    {
        public int instanceId;
        public float posX;
        public float posY;
    }
    
    public class CommandStack
    {
        private List<Command> _stack = new List<Command>();
        private readonly int _size;

        public CommandStack(int size)
        {
            _size = size;
        }

        public CommandStack()
        {
            _size = 100;
        }

        // �����
        public void Redo()
        {
            
        }
        
        // ���
        public Command Undo()
        {
            if (_stack.Count < 1) return null;
            
            var command = _stack[^1];
            _stack.RemoveAt(_stack.Count-1);
            return command;
        }

        public void Push(Command com)
        {
            if (_stack.Count > _size)
            {
                // ���� ������ ����� ����
                _stack.RemoveAt(0);
            }
            _stack.Add(com);
        }
    }
}