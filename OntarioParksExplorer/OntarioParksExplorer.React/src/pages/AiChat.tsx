import { useState, useEffect, useRef } from 'react';
import { chat } from '../services/api';
import { ChatMessage } from '../types';

export default function AiChat() {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!input.trim()) return;

    const userMessage = input.trim();
    setInput('');
    setError(null);

    // Add user message to chat
    const newMessages: ChatMessage[] = [
      ...messages,
      { role: 'user', content: userMessage }
    ];
    setMessages(newMessages);

    setLoading(true);
    try {
      const response = await chat(userMessage, messages);
      
      // Add AI response to chat
      setMessages([
        ...newMessages,
        { role: 'assistant', content: response }
      ]);
    } catch (err) {
      setError('Failed to get response. Please try again.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleClear = () => {
    setMessages([]);
    setError(null);
  };

  return (
    <div className="page chat-page">
      <div className="chat-header">
        <div>
          <h1>💬 AI Park Assistant</h1>
          <p className="page-description">
            Ask me anything about Ontario parks!
          </p>
        </div>
        {messages.length > 0 && (
          <button onClick={handleClear} className="btn btn-secondary">
            Clear Chat
          </button>
        )}
      </div>

      <div className="chat-container">
        <div className="chat-messages">
          {messages.length === 0 && (
            <div className="chat-welcome">
              <h2>👋 Welcome!</h2>
              <p>I'm here to help you explore Ontario's parks. You can ask me about:</p>
              <ul>
                <li>🏞️ Specific parks and their features</li>
                <li>🎯 Activity recommendations</li>
                <li>📍 Park locations and regions</li>
                <li>🗓️ Best times to visit</li>
                <li>ℹ️ General park information</li>
              </ul>
            </div>
          )}

          {messages.map((message, idx) => (
            <div
              key={idx}
              className={`chat-message ${message.role === 'user' ? 'user' : 'assistant'}`}
            >
              <div className="message-avatar">
                {message.role === 'user' ? '👤' : '🤖'}
              </div>
              <div className="message-content">
                {message.content}
              </div>
            </div>
          ))}

          {loading && (
            <div className="chat-message assistant">
              <div className="message-avatar">🤖</div>
              <div className="message-content loading-dots">
                <span>.</span><span>.</span><span>.</span>
              </div>
            </div>
          )}

          <div ref={messagesEndRef} />
        </div>

        {error && <div className="error-message" role="alert">{error}</div>}

        <form onSubmit={handleSubmit} className="chat-input-form">
          <label htmlFor="chat-input" className="visually-hidden">Your message</label>
          <input
            id="chat-input"
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Ask me about Ontario parks..."
            className="chat-input"
            disabled={loading}
            aria-label="Chat message input"
          />
          <button
            type="submit"
            className="btn btn-primary"
            disabled={loading || !input.trim()}
            aria-label="Send message"
          >
            Send
          </button>
        </form>
      </div>
    </div>
  );
}
