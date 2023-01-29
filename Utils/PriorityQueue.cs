using System;
using System.Collections.Generic;

namespace Utils
{
    // IComparable 인터페이스를 상속한 타입만 사용 가능 (제너릭을 위한 장치)
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _heap = new List<T>();
        
        // 속도는 트리의 높이에 영향을 받는데 높이는 Log2(N) ※ N => _heap 총 길이
        // 즉 O(n) => Log2(N)
        public void Push(T data)
        {
            _heap.Add(data);
            int now = _heap.Count - 1;
            
            // 자기 위의 노드가 자기보다 값이 낮으면 자리를 바꾼다. 
            while (now > 0)
            {
                int next = (now - 1) / 2; // 나의 부모의 index
                // 내가 더 작으니까 그대로 실패
                if (_heap[now].CompareTo(_heap[next]) < 0) break;
                // A.CompareTo(B) => A > B = 1
                // A.CompareTo(B) => A == B = 0
                // A.CompareTo(B) => A < B = -1
                
                //자리를 바꾼다. 
                (_heap[now], _heap[next]) = (_heap[next], _heap[now]);
                
                // 검사 위치를 이동한다.
                now = next;
            }
        }

        public T Pop()
        {
            // 반환할 데이터를 따로 저장
            T ret = _heap[0];
            int lastIndex = _heap.Count - 1; // 마지막 노드 index
            
            // 맨 마지막 데이터를 루트로 이동한다.
            {
                _heap[0] = _heap[lastIndex];
                // 마지막인 데이터를 루트로 올렸으니까 마지막 데이터를 지워준다.
                _heap.RemoveAt(lastIndex);
                lastIndex--;
            }

            // 맨 마지막 데이터를 올려준 상태에서 역으로 크기검사 시작
            // 자신의 자식(양쪽)이 나보다 클때 자리를 교환 
            // 만약 둘다 크다면 왼쪽을 선택
            int now = 0;
            while (true){
                // 자신의 위치에서 자식들 체크 
                int left = 2 * now + 1; // 왼쪽
                int right = 2 * now + 2; // 오른쪽

                int next = now;

                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동할 준비
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0 ) next = left;

                // 오른쪽 값이 현재값(왼쪽 이동 포함 )보다 크면 오른쪽으로 이동할 준비
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0) next = right;

                // 왼족 오른쪽 모두 현재값보다 작으면 종료
                if(next == now) break;
                
                // 두 값을 교체한다.
                (_heap[now], _heap[next]) = (_heap[next], _heap[now]);

                // 검사 위치를 이동한다.
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