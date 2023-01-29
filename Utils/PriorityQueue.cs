using System;
using System.Collections.Generic;

namespace Utils
{
    // IComparable �������̽��� ����� Ÿ�Ը� ��� ���� (���ʸ��� ���� ��ġ)
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _heap = new List<T>();
        
        // �ӵ��� Ʈ���� ���̿� ������ �޴µ� ���̴� Log2(N) �� N => _heap �� ����
        // �� O(n) => Log2(N)
        public void Push(T data)
        {
            _heap.Add(data);
            int now = _heap.Count - 1;
            
            // �ڱ� ���� ��尡 �ڱ⺸�� ���� ������ �ڸ��� �ٲ۴�. 
            while (now > 0)
            {
                int next = (now - 1) / 2; // ���� �θ��� index
                // ���� �� �����ϱ� �״�� ����
                if (_heap[now].CompareTo(_heap[next]) < 0) break;
                // A.CompareTo(B) => A > B = 1
                // A.CompareTo(B) => A == B = 0
                // A.CompareTo(B) => A < B = -1
                
                //�ڸ��� �ٲ۴�. 
                (_heap[now], _heap[next]) = (_heap[next], _heap[now]);
                
                // �˻� ��ġ�� �̵��Ѵ�.
                now = next;
            }
        }

        public T Pop()
        {
            // ��ȯ�� �����͸� ���� ����
            T ret = _heap[0];
            int lastIndex = _heap.Count - 1; // ������ ��� index
            
            // �� ������ �����͸� ��Ʈ�� �̵��Ѵ�.
            {
                _heap[0] = _heap[lastIndex];
                // �������� �����͸� ��Ʈ�� �÷����ϱ� ������ �����͸� �����ش�.
                _heap.RemoveAt(lastIndex);
                lastIndex--;
            }

            // �� ������ �����͸� �÷��� ���¿��� ������ ũ��˻� ����
            // �ڽ��� �ڽ�(����)�� ������ Ŭ�� �ڸ��� ��ȯ 
            // ���� �Ѵ� ũ�ٸ� ������ ����
            int now = 0;
            while (true){
                // �ڽ��� ��ġ���� �ڽĵ� üũ 
                int left = 2 * now + 1; // ����
                int right = 2 * now + 2; // ������

                int next = now;

                // ���ʰ��� ���簪���� ũ��, �������� �̵��� �غ�
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0 ) next = left;

                // ������ ���� ���簪(���� �̵� ���� )���� ũ�� ���������� �̵��� �غ�
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0) next = right;

                // ���� ������ ��� ���簪���� ������ ����
                if(next == now) break;
                
                // �� ���� ��ü�Ѵ�.
                (_heap[now], _heap[next]) = (_heap[next], _heap[now]);

                // �˻� ��ġ�� �̵��Ѵ�.
                now = next;
            }
            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }
}