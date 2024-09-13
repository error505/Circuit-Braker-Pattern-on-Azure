import React, { useState } from 'react';
import axios from 'axios';

const ApiCaller: React.FC = () => {
  const [inputData, setInputData] = useState<string>('');
  const [responseData, setResponseData] = useState<string>('');
  const [error, setError] = useState<string>('');

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setInputData(event.target.value);
  };

  const callApi = async () => {
    try {
      setError('');
      const response = await axios.post(`${process.env.REACT_APP_API_URL}/api/perform-operation`, { data: inputData });
      setResponseData(response.data);
    } catch (err: any) {
      setError(err.response ? err.response.data : 'An error occurred while calling the API.');
    }
  };

  return (
    <div style={{ padding: '20px' }}>
      <h2>API Caller</h2>
      <input
        type="text"
        value={inputData}
        onChange={handleInputChange}
        placeholder="Enter data to send"
        style={{ width: '300px', padding: '10px', marginBottom: '10px' }}
      />
      <br />
      <button onClick={callApi} style={{ padding: '10px 20px', cursor: 'pointer' }}>
        Call API
      </button>

      {responseData && (
        <div style={{ marginTop: '20px' }}>
          <h3>Response:</h3>
          <p>{responseData}</p>
        </div>
      )}

      {error && (
        <div style={{ marginTop: '20px', color: 'red' }}>
          <h3>Error:</h3>
          <p>{error}</p>
        </div>
      )}
    </div>
  );
};

export default ApiCaller;
