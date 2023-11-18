using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BGMChart
{
    public class ChatViewModel
    {
        // 텍스트 뜨는지 참고용
        public ObservableCollection<ConversationMessages> Messages { get; set; } = new ObservableCollection<ConversationMessages>();
    }
}

    // 텍스트 관련
    public class ConversationMessages
    {
        // 메세지 보낸지/받은지 확인
        public string MessageStatus { get; set; }

        // 유저 이름
        public string Username { get; set; }

        // 메세지
        public string Message { get; set; }
    }