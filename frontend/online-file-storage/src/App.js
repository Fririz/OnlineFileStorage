// Не забудь импортировать useState и axios
import { useState } from 'react';
import axios from 'axios';

// Твой компонент RegisterForm
function RegisterForm({ onLoginSuccess }) {
  // 1. Создаем "состояние" (state) для каждого поля ввода
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState(null); // Для отображения ошибок

  // 2. Функция, которая вызывается при отправке формы
  const handleSubmit = async (event) => {
    // Предотвращаем стандартное поведение формы (перезагрузку страницы)
    event.preventDefault();
    setError(null); // Сбрасываем старые ошибки

    try {
      // 3. Отправляем POST запрос на Ocelot Gateway
      const response = await axios.post('http://localhost:6000/api/auth/register', 
      {
        // Тело запроса (body)
        username: username,
        password: password
      }, 
      {
        // Конфиг запроса
        // !!! ЭТО КРИТИЧЕСКИ ВАЖНО !!!
        // Разрешает браузеру отправлять и получать Cookies (учетные данные)
        withCredentials: true 
      });

      // 4. ЕСЛИ ЗАПРОС ПРОШЕЛ УСПЕШНО (статус 200-299):
      // Бэкенд УЖЕ УСТАНОВИЛ HttpOnly Cookie в ответе.
      
      // Нам НЕ НУЖНО вручную сохранять токен. Браузер сделал это за нас.

      // 5. Просто сообщаем родительскому App.js, что мы вошли
      onLoginSuccess();

    } catch (err) {
      // 6. Если бэкенд вернул ошибку (400, 404, 500...)
      console.error('Ошибка регистрации:', err);
      
      // Попытаемся показать более осмысленную ошибку от бэкенда
      const message = err.response?.data?.message || 'Не удалось зарегистрироваться. Проверьте данные.';
      setError(message);
    }
  };

  return (
    <div>
      <h2>Register</h2>
      {/* 7. Вешаем наш обработчик на 'onSubmit' формы */}
      <form onSubmit={handleSubmit}>
        <input 
          type="text" 
          placeholder="Username"
          value={username} // Привязываем к состоянию
          onChange={e => setUsername(e.target.value)} // Обновляем состояние при вводе
        />
        <input 
          type="password" 
          placeholder="Password"
          value={password} // Привязываем к состоянию
          onChange={e => setPassword(e.target.value)} // Обновляем состояние при вводе
        />
        <button type="submit">Register</button>

        {/* Показываем ошибку, если она есть */}
        {error && <p style={{ color: 'red' }}>{error}</p>}
      </form>
    </div>
  );
}

export default RegisterForm;
