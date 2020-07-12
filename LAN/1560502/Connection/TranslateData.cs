using System;

// Lưu các thông tin được gửi đi khi dịch chuyển mảnh trong form Main hoặc Remote
// dữ liệu được bắn mỗi khi có một mảnh nào đó dịch chuyển

namespace GhepHinh
{
    [Serializable]
    public class TranslateData
    {
        public int left, top;
        public int x, y;

        public TranslateData(int left, int top, int x = 0, int y = 0)
        {
            this.left = left;
            this.top = top;
            this.x = x;
            this.y = y;
        }
    }
}
